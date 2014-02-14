using System.Data.Entity;
using System.Threading.Tasks;
using TrackableEntities.EF6;

namespace TrackableEntities.Patterns.EF6
{
    /// <summary>
    /// Generic repository with basic operations.
    /// </summary>
    /// <typeparam name="TEntity">Entity type for the repository.</typeparam>
    public abstract class Repository<TEntity> : IRepository<TEntity>
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
        /// <returns>A task that represents the asynchronous find operation. The task result contains the entity found, or null.</returns>
        public virtual async Task<TEntity> Find(params object[] keyValues)
        {
            return await Set.FindAsync(keyValues);
        }

        /// <summary>
        /// Inserts a new entity into the repository.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public virtual void Insert(TEntity entity)
        {
            Set.Add(entity);
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
        /// Removes an entity from the respository.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result will be alse if the entity does not exist in the repository, or true if successfully deleted.</returns>
        public virtual async Task<bool> Delete(params object[] keyValues)
        {
            var entity = await Find(keyValues);
            if (entity == null) return false;

            Set.Attach(entity);
            Set.Remove(entity);
            return true;
        }
    }
}
