using System;
using System.Collections.Generic;

namespace TrackableEntities.Patterns
{
    /// <summary>
    /// Generic repository interface with basic operations.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Removes an entity from the respository.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>False if the entity does not exist in the repository, or true if successfully deleted.</returns>
        bool Delete(params object[] keyValues);

        /// <summary>
        /// Load related entities for an object graph.
        /// </summary>
        /// <param name="entity">Entity on which related entities are loaded.</param>
        void LoadRelatedEntities(TEntity entity);

        /// <summary>
        /// Load related entities for more than one object graph.
        /// </summary>
        /// <param name="entities">Entities on which related entities are loaded.</param>
        void LoadRelatedEntities(IEnumerable<TEntity> entities);
    }
}
