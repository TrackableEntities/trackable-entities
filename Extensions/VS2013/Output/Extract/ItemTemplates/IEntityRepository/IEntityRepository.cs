using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using $baseNamespace$.Entities.Models;

namespace $rootnamespace$
{
    public interface $safeitemname$ : IRepository<$entityName$>, IRepositoryAsync<$entityName$>
    {
        Task<IEnumerable<$entityName$>> Get$entitySetName$();
        Task<$entityName$> Get$entityName$(int id);
        Task<bool> Delete$entityName$(int id);
    }
}
