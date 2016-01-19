using System;
#if EF_6
using System.Data.Entity;
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
    /// Interceptor for setting explicitly the state of an entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    public class StateChangeInterceptor<TEntity> : IStateInterceptor where TEntity : class, ITrackable
    {
        private readonly Func<TEntity, RelationshipType, EntityState?> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateChangeInterceptor"/> class.
        /// </summary>
        public StateChangeInterceptor(Func<TEntity, RelationshipType, EntityState?> selector)
        {
            this.selector = selector;
        }

        /// <summary>
        /// Gets state of <paramref name="item"/> based on <paramref name="relationshipType"/>.
        /// </summary>
        /// <param name="item">Current item.</param>
        /// <param name="relationshipType">Relationship of current item.</param>
        /// <returns>State of <paramref name="item"/> based on <paramref name="relationshipType"/>.</returns>
        public EntityState? GetEntityState(ITrackable item, RelationshipType relationshipType)
        {
            var entity = item as TEntity;
            return entity != null ? selector(entity, relationshipType) : null;
        }
    }
}