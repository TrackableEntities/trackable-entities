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
    public class StateChangeInterceptor<TEntity> : IInterceptor where TEntity : class, ITrackable
    {
        private readonly Func<TEntity, RelationshipType, EntityState?> selector;

        public StateChangeInterceptor(Func<TEntity, RelationshipType, EntityState?> selector)
        {
            this.selector = selector;
        }

        internal EntityState? GetEntityState(ITrackable item, RelationshipType relationshipType)
        {
            var entity = item as TEntity;
            return entity != null ? this.selector(entity, relationshipType) : null;
        }
    }
}