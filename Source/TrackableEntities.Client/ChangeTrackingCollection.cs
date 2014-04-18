using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

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
                // Get notified when an item in the collection has changed
                foreach (TEntity item in this)
                {
                    // Property change notification
                    if (value) item.PropertyChanged += OnPropertyChanged;
                    else item.PropertyChanged -= OnPropertyChanged;

                    // Enable tracking on trackable collection properties
                    item.SetTracking(value, Parent);

                    // Set entity identifier
                    item.SetEntityIdentifier();
                }
                _tracking = value;
            }
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
                if (entity != null
                    && e.PropertyName != Constants.TrackingProperties.TrackingState
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
                item.SetEntityIdentifier();

                // Listen for property changes
                item.PropertyChanged += OnPropertyChanged;

                // Enable tracking on trackable properties
                item.SetTracking(Tracking, Parent);

                // Mark item and trackable collection properties
                item.SetState(TrackingState.Added, Parent, this);

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

                // Remove modified properties
                item.ModifiedProperties = null;
                item.SetModifiedProperties(null, Parent);

                // Stop listening for property changes
                item.PropertyChanged -= OnPropertyChanged;

                // Disable tracking on trackable properties
                item.SetTracking(false, Parent);

                // Mark item and trackable collection properties
                item.SetState(TrackingState.Deleted, Parent, this);

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

            // Clone items in change tracker
            var items = this.Select(e => e.Clone<TEntity>()).ToList();

            // Remove deletes
            this.RemoveRestoredDeletes();

            // Get changed items
            List<TEntity> entities = items.GetChanges(null).Cast<TEntity>().ToList();

            // Return new change tracking collection with tracking disabled
            return new ChangeTrackingCollection<TEntity>(entities, true);
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
            foreach (var prop in typeof(TEntity).GetProperties())
            {
                var updatedItems = prop.GetValue(updatedItem, null) as IList;
                var originalItems = prop.GetValue(originalItem, null) as IList;
                if (originalItems != null && updatedItems != null)
                {
                    foreach (object item in originalItems)
                    {
                        var origTrackable = item as ITrackable;
                        if (origTrackable != null && origTrackable.TrackingState == TrackingState.Unchanged)
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

        private void FixUpParentReference(object child, object parent, bool isTracking)
        {
            foreach (var prop in child.GetType().GetProperties())
            {
                if (prop.PropertyType == parent.GetType())
                {
                    var childParent = prop.GetValue(child, null);
                    if (childParent != null && !ReferenceEquals(childParent, parent))
                    {
                        Tracking = false;
                        prop.SetValue(child, parent, null);
                        Tracking = isTracking;
                    }
                }
            }
        }
    }
}
