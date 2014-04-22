using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using TrackableEntities.EF6;

namespace TrackableEntities.Patterns.EF6
{
    /// <summary>
    /// Generic repository with basic operations.
    /// </summary>
    /// <typeparam name="TEntity">Entity type for the repository.</typeparam>
    public abstract class Repository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity>
        where TEntity : class, ITrackable
    {
        /// <summary>
        /// Creates a new Repository.
        /// </summary>
        protected Repository() { }

        /// <summary>
        /// Creates a new Repository.
        /// </summary>
        /// <param name="context">Entity Framework DbContext-derived class.</param>
        protected Repository(DbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets and sets the DbContext for the repository.
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// Gets the DbSet for the respository
        /// </summary>
        protected DbSet<TEntity> Set
        {
            get
            {
                return Context.Set<TEntity>();
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        public virtual TEntity Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public virtual void Insert(TEntity entity)
        {
            entity.TrackingState = TrackingState.Added;
            Context.ApplyChanges(entity);
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        public virtual void Update(TEntity entity)
        {
            Context.ApplyChanges(entity);
        }

        /// <summary>
        /// <para>Removes an entity from the respository.</para>
        /// <para>Override to include child entities in delete operation.</para>
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>False if the entity does not exist in the repository, or true if successfully deleted.</returns>
        public virtual bool Delete(params object[] keyValues)
        {
            var entity = Find(keyValues);
            if (entity == null) return false;
            ApplyDelete(entity);
            return true;
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await FindAsync(CancellationToken.None, keyValues);
        }

        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await Set.FindAsync(cancellationToken, keyValues);
        }

        /// <summary>
        /// <para>Removes an entity from the respository.</para>
        /// <para>Override to include child entities in delete operation.</para>
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        /// <summary>
        /// <para>Removes an entity from the respository.</para>
        /// <para>Override to include child entities in delete operation.</para>
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);
            if (entity == null) return false;
            ApplyDelete(entity);
            return true;
        }

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        public virtual void LoadRelatedEntities(TEntity entity)
        {
            Context.LoadRelatedEntities(entity);
        }

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task LoadRelatedEntitiesAsync(TEntity entity)
        {
            await Context.LoadRelatedEntitiesAsync(entity);
        }

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task LoadRelatedEntitiesAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await Context.LoadRelatedEntitiesAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        public void LoadRelatedEntities(IEnumerable<TEntity> entities)
        {
            Context.LoadRelatedEntities(entities);
        }

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task LoadRelatedEntitiesAsync(IEnumerable<TEntity> entities)
        {
            await Context.LoadRelatedEntitiesAsync(entities);
        }

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task LoadRelatedEntitiesAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await Context.LoadRelatedEntitiesAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Mark entity as deleted and apply changes to context.
        /// </summary>
        /// <param name="entity">Entity which is marked as deleted.</param>
        protected void ApplyDelete(TEntity entity)
        {
            entity.TrackingState = TrackingState.Deleted;
            Context.ApplyChanges(entity);
        }
    }
}
