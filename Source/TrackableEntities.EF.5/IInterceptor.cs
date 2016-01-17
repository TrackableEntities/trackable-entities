#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Interface implemented by interceptors added to <see cref="DbContextExtensions.ApplyChanges(System.Data.Entity.DbContext,ITrackable)"/>.
    /// </summary>
    public interface IInterceptor
    {
    }
}