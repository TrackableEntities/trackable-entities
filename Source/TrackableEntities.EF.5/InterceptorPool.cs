using System.Collections.Generic;
using System.Data.Entity;

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Pool of interceptors.
    /// </summary>
    public class InterceptorPool
    {
        internal DbContext DbContext { get; private set; }
        internal IList<IInterceptor> Interceptors { get; private set; }

        internal InterceptorPool(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.Interceptors = new List<IInterceptor>();
        }
    }
}