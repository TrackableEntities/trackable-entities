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
    /// Extension methods for <see cref="DbContext"/> and <see cref="InterceptorPool"/> with fluent API allowing
    /// to add inteceptors (instances of <see cref="IStateInterceptor"/>) to <see cref="DbContextExtensions.ApplyChanges(DbContext, ITrackable)"/>.
    /// </summary>
    public static class InterceptorExtensions
    {
        /// <summary>
        /// Injects interceptor implementing <see cref="IStateInterceptor"/> intto <see cref="DbContextExtensions.ApplyChanges(DbContext, ITrackable)"/>.
        /// </summary>
        /// <param name="dbContext"><see cref="DbContext"/> used to query and save changes to a database</param>
        /// <param name="interceptor">Instance of <see cref="IStateInterceptor"/></param>
        public static InterceptorPool WithInterceptor(this DbContext dbContext, IStateInterceptor interceptor)
        {
            var pool = new InterceptorPool(dbContext);
            return pool.WithInterceptor(interceptor);
        }

        /// <summary>
        /// Injects interceptor implementing <see cref="IStateInterceptor"/> into <see cref="DbContextExtensions.ApplyChanges(DbContext, ITrackable)"/>.
        /// </summary>
        /// <param name="pool">Pool of interceptors.</param>
        /// <param name="interceptor">Instance of <see cref="IStateInterceptor"/></param>
        public static InterceptorPool WithInterceptor(this InterceptorPool pool, IStateInterceptor interceptor)
        {
            pool.Interceptors.Add(interceptor);
            return pool;
        }

        /// <summary>
        /// Injects interceptor for setting explicitly the state of an entity into <see cref="DbContextExtensions.ApplyChanges(DbContext, ITrackable)"/>.
        /// </summary>
        /// <param name="dbContext"><see cref="DbContext"/> used to query and save changes to a database</param>
        /// <param name="stateSelector">Used for setting state of entity.</param>
        public static InterceptorPool WithStateChangeInterceptor<TEntity>(this DbContext dbContext,
            Func<TEntity, RelationshipType, EntityState?> stateSelector)
            where TEntity : class, ITrackable
        {
            var pool = new InterceptorPool(dbContext);
            return pool.WithStateChangeInterceptor(stateSelector);
        }

        /// <summary>
        /// Injects interceptor for setting explicitly the state of an entity into <see cref="DbContextExtensions.ApplyChanges(DbContext, ITrackable)"/>.
        /// </summary>
        /// <param name="pool">Pool of interceptors</param>
        /// <param name="stateSelector">Used for setting state of entity</param>
        public static InterceptorPool WithStateChangeInterceptor<TEntity>(this InterceptorPool pool,
            Func<TEntity, RelationshipType, EntityState?> stateSelector)
            where TEntity : class, ITrackable
        {
            pool.Interceptors.Add(new StateChangeInterceptor<TEntity>(stateSelector));
            return pool;
        }
    }
}