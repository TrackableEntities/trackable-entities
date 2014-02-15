using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackableEntities.Patterns;
using $entitiesNamespace$.Models;

namespace $rootnamespace$
{
    // NOTE: First add Entity Repository Interface in Service.Persistence project
    
    public class $entityName$Repository : Repository<$entityName$>, I$entityName$Repository
    {
        // TODO: Match Database Context Interface type
        private readonly IDatabaseContext _context;

        // TODO: Match Database Context Interface type
        public OrderRepository(IDatabaseContext context) : 
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
                 .SingleOrDefaultAsync(t => t.Id == id); // TODO: Use primary key
            return entity;
        }

        public async Task<bool> Delete$entityName$(int id)
        {
            $entityName$ entity = await Get$entityName$(id);
            if (entity == null) return false;
            Set.Attach(entity);
            Set.Remove(entity);

            // TODO: Remove child entities
            return true;
        }
        
        // TODO: Add methods to load related entities if needed
    }
}
