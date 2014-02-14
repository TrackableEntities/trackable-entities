using System;
using System.Threading.Tasks;

namespace TrackableEntities.Patterns
{
    /// <summary>
    /// Generic repository interface used to abstract away persistence stack.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Finds an entity with the given primary key values. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        Task<TEntity> Find(params object[] keyValues);

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
        /// <returns>A task that represents the asynchronous delete operation. The task result will be alse if the entity does not exist in the repository, or true if successfully deleted.</returns>
        Task<bool> Delete(params object[] keyValues);
    }
}
