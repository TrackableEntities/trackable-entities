using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrackableEntities.Patterns
{
    /// <summary>
    /// Generic repository interface with basic asynchronous operations.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepositoryAsync<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        Task<TEntity> FindAsync(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);

        /// <summary>
        /// Removes an entity from the respository.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task<bool> DeleteAsync(params object[] keyValues);

        /// <summary>
        /// Removes an entity from the respository.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task LoadRelatedEntitiesAsync(TEntity entity);

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task LoadRelatedEntitiesAsync(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task LoadRelatedEntitiesAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be false if the entity does not exist in the repository, or true if successfully removed.</returns>
        Task LoadRelatedEntitiesAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    }
}
