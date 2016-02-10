using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
        internal IEnumerable<IStateInterceptor> Interceptors { get; private set; }

        internal InterceptorPool(DbContext dbContext, IStateInterceptor interceptor)
            : this(dbContext, Enumerable.Empty<IStateInterceptor>(), interceptor)
        { }

        internal InterceptorPool(InterceptorPool previousPool, IStateInterceptor interceptor)
            : this(previousPool.DbContext, previousPool.Interceptors, interceptor)
        { }

        private InterceptorPool(DbContext dbContext, IEnumerable<IStateInterceptor> previousInterceptors, IStateInterceptor interceptor)
        {
            DbContext = dbContext;
            Interceptors = previousInterceptors.Union(Enumerable.Repeat(interceptor, 1));
        }
    }
}