 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;
using TrackableEntities.EF6;
using $rootNamespace$.Models;

// NOTE: Add Trackable Entities EF Nuget package, then reference 
//       Trackable Service Entities project and System.ServiceModel.

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
                .ToListAsync();
            return entities;
        }

        public async Task<$entityName$> Get$entityName$(int id)
        {
            $entityName$ entity = await _dbContext.$entitySetName$
                .SingleOrDefaultAsync(x => /* TODO: Insert key field */ x.Id == id);
            return entity;
        }

        public async Task<$entityName$> Update$entityName$($entityName$ entity)
        {
            try
            {
                _dbContext.ApplyChanges(entity);
                await _dbContext.SaveChangesAsync();
                entity.AcceptChanges();
                return entity;
            }
            catch (DbUpdateConcurrencyException updateEx)
            {
                throw new FaultException(updateEx.Message);
            }
        }

        public async Task<$entityName$> Create$entityName$($entityName$ entity)
        {
            _dbContext.$entitySetName$.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        // TODO: Accept entity concurrency property (rowversion)
        public async Task<bool> Delete$entityName$(int id)
        {
            $entityName$ entity = await _dbContext.$entitySetName$
                .SingleOrDefaultAsync(x => /* TODO: Insert key field */ x.Id == id);
            if (entity == null)
                return false;

            try
            {
                _dbContext.$entitySetName$.Attach(entity);
                _dbContext.$entitySetName$.Remove(entity);
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
