using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using $entitiesNamespace$.Models;

namespace $rootnamespace$
{
    public interface $safeitemname$ : IRepository<$entityName$>, IRepositoryAsync<$entityName$>
    {
        Task<IEnumerable<$entityName$>> Get$entitySetName$();
        Task<$entityName$> GetOrder(int id);
        Task<bool> DeleteOrder(int id);
    }
}
