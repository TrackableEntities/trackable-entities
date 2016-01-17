using System;
using System.Collections.Generic;
using System.Data.Entity;
#if EF_6
#else
using System.Data;
#endif

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Extension methods for <see cref="DbContext"/> with fluent API allowing
    /// to add inteceptors (instances of <see cref="IInterceptor"/>) to <see cref="DbContextExtensions.ApplyChanges(DbContext,ITrackable)"/>.
    /// </summary>
    public static class InterceptorExtensions
    {
        /// <summary>
        /// Add any interceptor implementing <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="dbContext"><see cref="DbContext"/> used to query and save changes to a database</param>
        /// <param name="interceptor">Instance of <see cref="IInterceptor"/></param>
        public static InterceptorPool AddInterceptor(this DbContext dbContext, IInterceptor interceptor)
        {
            var pool = new InterceptorPool(dbContext);
            return pool.AddInterceptor(interceptor);
        }

        /// <summary>
        /// Add any interceptor implementing <see cref="IInterceptor"/>.
        /// </summary>
        /// <param name="pool">Pool of interceptors.</param>
        /// <param name="interceptor">Instance of <see cref="IInterceptor"/></param>
        public static InterceptorPool AddInterceptor(this InterceptorPool pool, IInterceptor interceptor)
        {
            pool.Interceptors.Add(interceptor);
            return pool;
        }

        /// <summary>
        /// Add interceptor for setting explicitly the state of an entity.
        /// </summary>
        /// <param name="dbContext"><see cref="DbContext"/> used to query and save changes to a database</param>
        /// <param name="stateSelector">Used for setting state of entity.</param>
        public static InterceptorPool AddStateChangeInterceptor<TEntity>(this DbContext dbContext,
            Func<TEntity, RelationshipType, EntityState?> stateSelector)
            where TEntity : class, ITrackable
        {
            var pool = new InterceptorPool(dbContext);
            return pool.AddStateChangeInterceptor(stateSelector);
        }

        /// <summary>
        /// Add interceptor for setting explicitly the state of an entity.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="stateSelector">Used for setting state of entity</param>
        public static InterceptorPool AddStateChangeInterceptor<TEntity>(this InterceptorPool pool,
            Func<TEntity, RelationshipType, EntityState?> stateSelector)
            where TEntity : class, ITrackable
        {
            pool.Interceptors.Add(new StateChangeInterceptor<TEntity>(stateSelector));
            return pool;
        }

        /// <summary>
        /// Update entity state on DbContext for an object graph.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this InterceptorPool pool, ITrackable item)
        {
            pool.DbContext.ApplyChanges(item, pool.Interceptors);
        }

        /// <summary>
        /// Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this InterceptorPool pool, IEnumerable<ITrackable> items)
        {
            pool.DbContext.ApplyChanges(items, pool.Interceptors);
        }
    }
}