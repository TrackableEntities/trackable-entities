using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using TrackableEntities.Patterns.EF6;
using $entitiesNamespace$;
using $baseNamespace$.Service.EF.Contexts;
using $baseNamespace$.Service.Persistence.Repositories;

namespace $rootnamespace$
{
    // NOTE: I$entityName$Repository will need to have been added to the Service.Persistence project
    
    public class $safeitemname$ : Repository<$entityName$>, I$entityName$Repository
    {
        // TODO: Match Database Context Interface type
        private readonly IDatabaseContext _context;

        // TODO: Match Database Context Interface type
        public $safeitemname$(IDatabaseContext context) : 
            base(context as DbContext)
        {
            _context = context;
        }

        public async Task<IEnumerable<$entityName$>> Get$entitySetName$()
        {
            // TODO: Add Includes for related entities if needed
            IEnumerable<$entityName$> entities = await _context.$entitySetName$
                .ToListAsync();
            return entities;
        }

        public async Task<$entityName$> Get$entityName$(int id)
        {
            // TODO: Add Includes for related entities if needed
            $entityName$ entity = await _context.$entitySetName$
                 .SingleOrDefaultAsync(t => t.$entityName$Id == id);
            return entity;
        }

        public async Task<bool> Delete$entityName$(int id)
        {
            // TODO: Add Includes for related entities if needed
            $entityName$ entity = await _context.$entitySetName$
                 .SingleOrDefaultAsync(t => t.$entityName$Id == id);
            if (entity == null) return false;
			ApplyDelete(entity);
            return true;
        }
    }
}
