using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using TrackableEntities.Common;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Collection responsible for tracking changes to entities.
    /// </summary>
    /// <typeparam name="TEntity">Trackable entity type</typeparam>
    public class ChangeTrackingCollection<TEntity> : ObservableCollection<TEntity>,
        ITrackingCollection<TEntity>, ITrackingCollection
        where TEntity : class, ITrackable, INotifyPropertyChanged
    {
        // Deleted entities cache
        readonly private Collection<TEntity> _deletedEntities = new Collection<TEntity>();

        /// <summary>
        /// Event for when an entity in the collection has changed its tracking state.
        /// </summary>
        public event EventHandler EntityChanged;

        /// <summary>
        /// Default contstructor with change-tracking disabled
        /// </summary>
        public ChangeTrackingCollection() : this(false) { }

        /// <summary>
        /// Change-tracking will not begin after entities are added, 
        /// unless tracking is enabled.
        /// </summary>
        /// <param name="enableTracking">Enable tracking after entities are added</param>
        public ChangeTrackingCollection(bool enableTracking)
        {
            // Initialize excluded properties
            ExcludedProperties = new List<string>();

            // Enable or disable tracking
            Tracking = enableTracking;
        }

        /// <summary>
        /// Constructor that accepts one or more entities.
        /// Change-tracking will begin after entities are added.
        /// </summary>
        /// <param name="entities">Entities being change-tracked</param>
        public ChangeTrackingCollection(params TEntity[] entities)
            : this(entities, false) { }

        /// <summary>
        /// Constructor that accepts a collection of entities.
        /// Change-tracking will begin after entities are added, 
        /// unless tracking is disabled.
        /// </summary>
        /// <param name="entities">Entities being change-tracked</param>
        /// <param name="disableTracking">Disable tracking after entities are added</param>
        public ChangeTrackingCollection(IEnumerable<TEntity> entities, bool disableTracking = false)
        {
            // Initialize excluded properties
            ExcludedProperties = new List<string>();
            
            // Add items to the change tracking list
            foreach (TEntity item in entities)
            {
                Add(item);
            }
            Tracking = !disableTracking;
        }

        /// <summary>
        /// Properties to exclude from change tracking.
        /// </summary>
        public IList<string> ExcludedProperties { get; private set; } 

        /// <summary>
        /// Turn change-tracking on and off.
        /// </summary>
        public bool Tracking
        {
            get
            {
                return _tracking;
            }
            set
            {
                SetTracking(value, new ObjectVisitationHelper(), false);
            }
        }

        /// <summary>
        /// For internal use.
        /// Turn change-tracking on and off with proper circular reference checking.
        /// </summary>
        public void SetTracking(bool value, ObjectVisitationHelper visitationHelper, bool oneToManyOnly)
        {
            ObjectVisitationHelper.EnsureCreated(ref visitationHelper);

            // Prevent endless recursion
            if (!visitationHelper.TryVisit(this)) return;

            // Get notified when an item in the collection has changed
            foreach (TEntity item in this)
            {
                // Prevent endless recursion
                if (!visitationHelper.TryVisit(item)) continue;

                // Property change notification
                if (value) item.PropertyChanged += OnPropertyChanged;
                else item.PropertyChanged -= OnPropertyChanged;

                // Enable tracking on trackable collection properties
                item.SetTracking(value, visitationHelper, oneToManyOnly);

                // Set entity identifier
                if (item is IIdentifiable)
                    ((IIdentifiable)item).SetEntityIdentifier();
            }
            _tracking = value;
        }
        private bool _tracking;

        /// <summary>
        /// ITrackable parent referencing items in this collection.
        /// </summary>
        public ITrackable Parent { get; set; }

        // Fired when an item in the collection has changed
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Tracking)
            {
                var entity = sender as ITrackable;
                if (entity == null) return;

                // Enable tracking on reference properties
                var prop = PortableReflectionHelper.Instance.GetProperty(entity.GetType(), e.PropertyName);
                if (prop != null && PortableReflectionHelper.Instance.IsAssignable(typeof(ITrackable), prop.PropertyType))
                {
                    ITrackingCollection refPropChangeTracker = entity.GetRefPropertyChangeTracker(e.PropertyName);
                    if (refPropChangeTracker != null)
                        refPropChangeTracker.Tracking = Tracking;
                    return;
                }

                if (e.PropertyName != Constants.TrackingProperties.TrackingState
                    && e.PropertyName != Constants.TrackingProperties.ModifiedProperties
                    && !ExcludedProperties.Contains(e.PropertyName))
                {
                    // If unchanged mark item as modified, fire EntityChanged event
                    if (entity.TrackingState == TrackingState.Unchanged)
                    {
                        entity.TrackingState = TrackingState.Modified;
                        if (EntityChanged != null) EntityChanged(this, EventArgs.Empty);
                    }

                    // Add prop to modified props, and fire EntityChanged event
                    if (entity.TrackingState == TrackingState.Unchanged
                        || entity.TrackingState == TrackingState.Modified)
                    {
                        if (entity.ModifiedProperties == null)
                            entity.ModifiedProperties = new List<string>();
                        if (!entity.ModifiedProperties.Contains(e.PropertyName))
                            entity.ModifiedProperties.Add(e.PropertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Insert item at specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted</param>
        /// <param name="item">Item to insert</param>
        protected override void InsertItem(int index, TEntity item)
        {
            if (Tracking)
            {
                // Set entity identifier
                if (item is IIdentifiable)
                    ((IIdentifiable)item).SetEntityIdentifier();

                // Listen for property changes
                item.PropertyChanged += OnPropertyChanged;

                // Exclude this collection and Parent entity (used in M-M relationships)
                // from recusive algorithms: SetTracking and SetState.
                var visitationHelper = new ObjectVisitationHelper(Parent);
                visitationHelper.TryVisit(this);

                // Enable tracking on trackable properties
                item.SetTracking(Tracking, visitationHelper.Clone());

                // Mark item and trackable collection properties
                item.SetState(TrackingState.Added, visitationHelper.Clone());

                // Fire EntityChanged event
                if (EntityChanged != null) EntityChanged(this, EventArgs.Empty);
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Remove item at specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be removed</param>
        protected override void RemoveItem(int index)
        {
            // Mark existing item as deleted, stop listening for property changes,
            // then fire EntityChanged event, and cache item.
            if (Tracking)
            {
                // Get item by index
                TEntity item = Items[index];

                // Exclude this collection and Parent entity (used in M-M relationships)
                // from recusive algorithms: SetModifiedProperties, SetTracking and SetState.
                var visitationHelper = new ObjectVisitationHelper(Parent);
                visitationHelper.TryVisit(this);

                // Remove modified properties
                item.ModifiedProperties = null;
                item.SetModifiedProperties(null, visitationHelper.Clone());

                // Stop listening for property changes
                item.PropertyChanged -= OnPropertyChanged;

                // Disable tracking on trackable properties
                item.SetTracking(false, visitationHelper.Clone(), true);

                // Mark item and trackable collection properties
                item.SetState(TrackingState.Deleted, visitationHelper.Clone());

                // Fire EntityChanged event
                if (EntityChanged != null) EntityChanged(this, EventArgs.Empty);

                // Cache deleted item if not added or already cached
                if (item.TrackingState != TrackingState.Added
                    && !_deletedEntities.Contains(item))
                    _deletedEntities.Add(item);
            }
            base.RemoveItem(index);
        }

        /// <summary>
        /// Get entities that have been added, modified or deleted, including child 
        /// collections with entities that have been added, modified or deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        public ChangeTrackingCollection<TEntity> GetChanges()
        {
            // Temporarily restore deletes
            this.RestoreDeletes();

            try
            {
                return CloneChangesHelper.GetChanges(this);
            }
            finally
            {
                // Remove deletes
                this.RemoveRestoredDeletes();
            }
        }

        private class CloneChangesHelper : DefaultContractResolver
        {
            private readonly ObjectVisitationHelper visitationHelper = new ObjectVisitationHelper();

            private readonly Dictionary<ITrackable, EntityChangedInfo> entityChangedInfos =
                new Dictionary<ITrackable, EntityChangedInfo>(ObjectReferenceEqualityComparer<ITrackable>.Default);

            public static ChangeTrackingCollection<TEntity> GetChanges(ChangeTrackingCollection<TEntity> source)
            {
                var helper = new CloneChangesHelper();
                var wrapper = new Wrapper() { Result = source };

                // Inspect the graph and collect entityChangedInfos
                helper.GetChanges(Enumerable.Repeat(wrapper, 1)).ToList();

                // Clone only changed items
                var clone = TrackableExtensions.CloneObject(wrapper, helper);

                return clone.Result;
            }

            private CloneChangesHelper()
            {
            }

            private class EntityChangedInfo
            {
                public readonly HashSet<PropertyInfo> RefNavPropUnchanged = new HashSet<PropertyInfo>();

                public readonly Dictionary<PropertyInfo, HashSet<ITrackable>> ColNavPropChangedEntities =
                    new Dictionary<PropertyInfo, HashSet<ITrackable>>();
            }

            private class Wrapper : ITrackable
            {
                [JsonProperty]
                public ChangeTrackingCollection<TEntity> Result { get; set; }

                public TrackingState TrackingState { get; set; }

                public ICollection<string> ModifiedProperties { get; set; }
            }

            private EntityChangedInfo EntityInfo(ITrackable entity)
            {
                EntityChangedInfo info;
                if (!entityChangedInfos.TryGetValue(entity, out info))
                { 
                    info = new EntityChangedInfo();
                    entityChangedInfos.Add(entity, info);
                }
                return info;
            }

            private bool IncludeReferenceProp(ITrackable entity, PropertyInfo propertyInfo)
            {
                EntityChangedInfo info;
                if (!entityChangedInfos.TryGetValue(entity, out info))
                    return true; // no excludes found for this entity

                return !info.RefNavPropUnchanged.Contains(propertyInfo);
            }

            private bool IncludeCollectionItem(ITrackable entity, PropertyInfo propertyInfo, ITrackable item)
            {
                EntityChangedInfo info;
                if (!entityChangedInfos.TryGetValue(entity, out info))
                    return true; // no excludes found for this entity

                HashSet<ITrackable> changedItems;
                if (!info.ColNavPropChangedEntities.TryGetValue(propertyInfo, out changedItems))
                    return false; // no items found for this collection

                return changedItems.Contains(item);
            }

            /// <summary>
            /// Get entities that have been added, modified or deleted, including trackable 
            /// reference and child entities.
            /// </summary>
            /// <param name="items">Collection of ITrackable objects</param>
            /// <returns>Collection containing only added, modified or deleted entities</returns>
            private IEnumerable<ITrackable> GetChanges(IEnumerable<ITrackable> items)
            {
                // Prevent endless recursion by collection
                if (!visitationHelper.TryVisit(items)) yield break;

                // Prevent endless recursion by item
                items = items.Where(i => visitationHelper.TryVisit(i)).ToList();

                // Iterate items in change-tracking collection
                foreach (ITrackable item in items)
                {
                    // Downstream changes flag
                    bool hasDownstreamChanges = false;

                    // Iterate entity properties
                    foreach (var navProp in item.GetNavigationProperties())
                    {
                        // Process 1-1 and M-1 properties
                        foreach (var refProp in navProp.AsReferenceProperty())
                        {
                            ITrackable trackableRef = refProp.EntityReference;

                            // if already visited and unchanged, set to null
                            if (visitationHelper.IsVisited(trackableRef))
                            {
                                if ((trackableRef.TrackingState == TrackingState.Unchanged
                                     || trackableRef.TrackingState == TrackingState.Deleted))
                                {
                                    EntityInfo(item).RefNavPropUnchanged.Add(refProp.Property);
                                }
                                continue;
                            }

                            // Get changed ref prop
                            ITrackingCollection refChangeTracker = item.GetRefPropertyChangeTracker(refProp.Property.Name);
                            if (refChangeTracker != null)
                            {
                                // Get downstream changes
                                IEnumerable<ITrackable> refPropItems = refChangeTracker.Cast<ITrackable>();
                                IEnumerable<ITrackable> refPropChanges = GetChanges(refPropItems);

                                // Set flag for downstream changes
                                bool hasLocalDownstreamChanges =
                                    refPropChanges.Any(t => t.TrackingState != TrackingState.Deleted) ||
                                    trackableRef.TrackingState == TrackingState.Added ||
                                    trackableRef.TrackingState == TrackingState.Modified;

                                // Set ref prop to null if unchanged or deleted
                                if (!hasLocalDownstreamChanges &&
                                    (trackableRef.TrackingState == TrackingState.Unchanged
                                        || trackableRef.TrackingState == TrackingState.Deleted))
                                {
                                    EntityInfo(item).RefNavPropUnchanged.Add(refProp.Property);
                                    continue;
                                }

                                // prevent overwrite of hasDownstreamChanges when return from recursion
                                hasDownstreamChanges |= hasLocalDownstreamChanges;
                            }
                        }

                        // Process 1-M and M-M properties
                        foreach (var colProp in navProp.AsCollectionProperty<IList>())
                        {
                            // Get changes on child collection
                            var trackingItems = colProp.EntityCollection;
                            if (trackingItems.Count > 0)
                            {
                                // Continue recursion if trackable hasn't been visited
                                if (!visitationHelper.IsVisited(trackingItems))
                                {
                                    // Get changes on child collection
                                    var trackingCollChanges = new HashSet<ITrackable>(
                                        GetChanges(trackingItems.Cast<ITrackable>()),
                                        ObjectReferenceEqualityComparer<ITrackable>.Default);

                                    // Set flag for downstream changes
                                    hasDownstreamChanges |= trackingCollChanges.Any();

                                    // Memorize only changed items of collection
                                    EntityInfo(item).ColNavPropChangedEntities[colProp.Property] = trackingCollChanges;
                                }
                            }
                        }
                    }

                    // Return item if it has changes
                    if (hasDownstreamChanges || item.TrackingState != TrackingState.Unchanged)
                        yield return item;
                }
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                property.ShouldSerialize =
                    instance =>
                    {
                        var entity = instance as ITrackable;
                        if (entity == null) return true;

                        EntityNavigationProperty np = entity
                            .GetNavigationProperties(false)
                            .FirstOrDefault(x => x.Property == member);
                        if (np == null) return true; // not a nav prop
                        if (np.ValueIsNull) return false; // nav prop is not initialized

                        foreach (var rp in np.AsReferenceProperty())
                        {
                            // don't serialize unchanged reference navigation props
                            return IncludeReferenceProp(entity, rp.Property);
                        }

                        // serialize collection navigation props
                        return true;
                    };

                // Inject the custom IValueProvider for entity collections which
                // returns only changed items
                property.ValueProvider = new CollectionValueProvider(this, member, property.ValueProvider);

                return property;
            }

            private class CollectionValueProvider : IValueProvider
            {
                private readonly IValueProvider _valueProvider;
                private readonly MemberInfo _member;
                private readonly CloneChangesHelper _resolver;
                private static readonly MethodInfo _genericCast;

                static CollectionValueProvider()
                {
                    Func<IEnumerable<ITrackable>, object> func = CastResult<int>;
                    _genericCast = PortableReflectionHelper.Instance.GetMethodInfo(func)
                        .GetGenericMethodDefinition();
                }

                public CollectionValueProvider(CloneChangesHelper resolver,
                    MemberInfo member,
                    IValueProvider valueProvider)
                {
                    _resolver = resolver;
                    _member = member;
                    _valueProvider = valueProvider;
                }

                public void SetValue(object target, object value)
                {
                    _valueProvider.SetValue(target, value);
                }

                public object GetValue(object target)
                {
                    var entity = target as ITrackable;

                    if (entity == null)
                        return _valueProvider.GetValue(target);

                    var cnp = entity
                        .GetNavigationProperties(false)
                        .OfCollectionType()
                        .FirstOrDefault(x => x.Property == _member);

                    if (cnp == null)
                        return _valueProvider.GetValue(target); // not a collection nav prop

                    if (cnp.ValueIsNull)
                        return null; // nav prop is not initialized

                    var items = cnp.EntityCollection.Where(i => _resolver.IncludeCollectionItem(entity, cnp.Property, i));

                    return _genericCast
                        .MakeGenericMethod(PortableReflectionHelper.Instance.GetGenericArguments(cnp.Property.PropertyType))
                        .Invoke(null, new[] { items });
                }

                private static object CastResult<T>(IEnumerable<ITrackable> items)
                {
                    return items.Cast<T>().ToList();
                }
            }
        }

        /// <summary>
        /// Get entities that have been added, modified or deleted.
        /// </summary>
        /// <param name="cachedDeletesOnly">True to return only cached deletes</param>
        /// <returns>Collection containing only changed entities</returns>
        ITrackingCollection ITrackingCollection.GetChanges(bool cachedDeletesOnly)
        {
            // Get removed deletes only
            if (cachedDeletesOnly)
                return new ChangeTrackingCollection<TEntity>(_deletedEntities, true);

            // Get changed items in this tracking collection
            var changes = (from existing in this
                           where existing.TrackingState != TrackingState.Unchanged
                           select existing)
                          .Union(_deletedEntities);
            return new ChangeTrackingCollection<TEntity>(changes, true);
        }

        /// <summary>
        /// Remove deleted entities which have been cached.
        /// </summary>
        void ITrackingCollection.RemoveCachedDeletes()
        {
            _deletedEntities.Clear();
        }

        /// <summary>
        /// <para>Merge changed child items into the original trackable entity. 
        /// This assumes GetChanges was called to update only changed items. 
        /// Non-recursive - only direct children will be merged.</para> 
        /// <code>Usage: MergeChanges(ref originalItem, updatedItem);</code>
        /// </summary>
        /// <param name="originalItem">Local entity containing unchanged child items.</param>
        /// <param name="updatedItem">Entity returned by an update operation.</param>
        [Obsolete("ChangeTrackingCollection.MergeChanges has been deprecated. Instead use ChangeTrackingCollection.MergeChanges.")]
        public void MergeChanges(ref TEntity originalItem, TEntity updatedItem)
        {
            // Get unchanged child entities
            foreach (var colProp in updatedItem.GetNavigationProperties().OfCollectionType<IList>())
            {
                var updatedItems = colProp.EntityCollection;
                var originalItems = originalItem.GetEntityCollectionProperty(colProp.Property).EntityCollection;
                if (originalItems != null)
                {
                    foreach (ITrackable origTrackable in originalItems)
                    {
                        if (origTrackable.TrackingState == TrackingState.Unchanged)
                        {
                            // Add unchanged original item to updated items
                            updatedItems.Add(origTrackable);
                            FixUpParentReference(origTrackable, updatedItem, Tracking);
                        }
                    }
                }
            }

            // Track updated item
            bool tracking = Tracking;
            Tracking = false;
            Remove(originalItem);
            Add(updatedItem);
            Tracking = tracking;

            // Set original item to updated item with unchanged items merged in
            originalItem = updatedItem;
        }

        private void FixUpParentReference(ITrackable child, ITrackable parent, bool isTracking)
        {
            foreach (var refProp in child.GetNavigationProperties()
                .OfReferenceType()
                .Where(rp => PortableReflectionHelper.Instance.IsAssignable(rp.Property.PropertyType, parent.GetType()))
                .Where(rp => !ReferenceEquals(rp.EntityReference, parent)))
                {
                    Tracking = false;
                    refProp.Property.SetValue(child, parent, null);
                    Tracking = isTracking;
                }
        }
    }
}
