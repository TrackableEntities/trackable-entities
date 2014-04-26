using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;
using TrackableEntities;
using TrackableEntities.EF6;
using TrackableEntities.Common;
using $baseNamespace$.Entities.Models;

// NOTE: Primary key name and/or type may need to be set manually.

namespace $rootnamespace$
{
    [ServiceContract(Namespace = "urn:trackable-entities:service")]
    public interface I$safeitemname$
    {
        [OperationContract]
        Task<IEnumerable<$entityName$>> Get$entitySetName$();

        [OperationContract]
        Task<$entityName$> Get$entityName$(int id);
        
        [OperationContract]
        Task<$entityName$> Update$entityName$($entityName$ entity);
        
        [OperationContract]
        Task<$entityName$> Create$entityName$($entityName$ entity);
        
        [OperationContract]
        Task<bool> Delete$entityName$(int id);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class $safeitemname$ : I$safeitemname$, IDisposable
    {
        private readonly $dbContextName$ _dbContext;

        public $safeitemname$()
        {
            _dbContext = new $dbContextName$();
        }

        public async Task<IEnumerable<$entityName$>> Get$entitySetName$()
        {
            IEnumerable<$entityName$> entities = await _dbContext.$entitySetName$
    			// TODO: Add Includes for reference and/or collection properties
                .ToListAsync();
            return entities;
        }

        public async Task<$entityName$> Get$entityName$(int id)
        {
            $entityName$ entity = await _dbContext.$entitySetName$
    			// TODO: Add Includes for reference and/or collection properties
                .SingleOrDefaultAsync(e => e.$entityName$Id == id);
            return entity;
        }

        public async Task<$entityName$> Create$entityName$($entityName$ entity)
        {
            entity.TrackingState = TrackingState.Added;
            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return entity;
        }

        public async Task<$entityName$> Update$entityName$($entityName$ entity)
        {
            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }

            await _dbContext.LoadRelatedEntitiesAsync(entity);
            entity.AcceptChanges();
            return entity;
        }

        public async Task<bool> Delete$entityName$(int id)
        {
            $entityName$ entity = await _dbContext.$entitySetName$
			    // TODO: Include child entities if any
                .SingleOrDefaultAsync(e => e.$entityName$Id == id);
            if (entity == null)
                return false;

            entity.TrackingState = TrackingState.Deleted;
            _dbContext.ApplyChanges(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }
        }

        public void Dispose()
        {
            var dispose = _dbContext as IDisposable;
            if (dispose != null)
            {
                _dbContext.Dispose();
            }
        }
    }
} 
