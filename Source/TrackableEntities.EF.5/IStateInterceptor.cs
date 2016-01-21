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
    /// Interface implemented by interceptors added to <see cref="DbContextExtensions.ApplyChanges(DbContext,ITrackable)"/>.
    /// </summary>
    public interface IStateInterceptor
    {
        /// <summary>
        /// Gets state of <paramref name="item"/> based on <paramref name="relationshipType"/>.
        /// </summary>
        /// <param name="item">Current item.</param>
        /// <param name="relationshipType">Relationship of current item.</param>
        /// <returns>State of <paramref name="item"/> based on <paramref name="relationshipType"/>.</returns>
        EntityState? GetEntityState(ITrackable item, RelationshipType? relationshipType);
    }
}