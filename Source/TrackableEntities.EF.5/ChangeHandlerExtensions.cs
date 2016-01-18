using System;
using System.Collections.Generic;
#if EF_6
using System.Data.Entity;
#else
using System.Data;
using System.Data.Entity;
#endif

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    public static class ChangeHandlerExtensions
    {
        public static IChangeHandler AddChangeHandler<TEntity>(this DbContext context,
            Func<TEntity, RelationshipType, EntityState?> changeHandler)
        {
            var handler = new ChangeHandler
            {
                DbContext = context,
                Handlers = new List<Delegate>
                {
                    changeHandler
                }
            };
            return handler;
        }

        public static IChangeHandler AddChangeHandler<TEntity>(this IChangeHandler handler,
            Func<TEntity, RelationshipType, EntityState?> changeHandler)
        {
            handler.Handlers.Add(changeHandler);
            return handler;
        }
    }
}
