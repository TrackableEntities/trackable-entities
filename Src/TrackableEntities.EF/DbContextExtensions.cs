using System;
using System.Collections;
using System.Data;
using System.Data.Entity;

namespace TrackableEntities.EF
{
    /// <summary>
    /// Extension methods for DbContext to persist trackable entities.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Recursively set entity state for DbContext entry.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this DbContext context, ITrackable item)
        {
            // Check for null args
            if (context == null)
                throw new ArgumentNullException("context");
            if (item == null)
                throw new ArgumentNullException("item");

            // Set state to Added on parent only
            if (item.TrackingState == TrackingState.Added)
            {
                context.Entry(item).State = EntityState.Added;
                return;
            }

            // Set state to Deleted on children and parent
            if (item.TrackingState == TrackingState.Deleted)
            {
                context.SetChanges(item, EntityState.Unchanged);
                context.SetChanges(item, EntityState.Deleted);
                return;
            }

            // Set modified properties
            if (item.TrackingState == TrackingState.Modified
                && item.ModifiedProperties != null
                && item.ModifiedProperties.Count > 0)
            {
                // Mark modified properties
                context.Entry(item).State = EntityState.Unchanged;
                foreach (var property in item.ModifiedProperties)
                    context.Entry(item).Property(property).IsModified = true;
            }
            // Set entity state
            else
            {
                context.Entry(item).State = item.TrackingState.ToEntityState();
            }

            // Set state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        var trackable = items[i] as ITrackable;
                        if (trackable != null)
                            context.ApplyChanges(trackable);
                    }
                }
            }
        }

        private static void SetChanges(this DbContext context,
            ITrackable item, EntityState state)
        {
            // Set state for child collections
            foreach (var prop in item.GetType().GetProperties())
            {
                var items = prop.GetValue(item, null) as IList;
                if (items != null)
                {
                    for (int i = items.Count - 1; i > -1; i--)
                    {
                        var trackable = items[i] as ITrackable;
                        if (trackable != null)
                            context.SetChanges(trackable, state);
                    }
                }
            }

            // Set entity state
            context.Entry(item).State = state;
        }

        private static EntityState ToEntityState(this TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Added:
                    return EntityState.Added;
                case TrackingState.Modified:
                    return EntityState.Modified;
                case TrackingState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}
