using System;
using System.Collections;
using System.Data.Entity;
using System.Threading.Tasks;

namespace TrackableEntities.Patterns.EF6
{
    /// <summary>
    /// General unit of work for committing changes across one or more repositories. 
    /// Inherit from this class to supply a specific DbContext. 
    /// Add a property for each respository for which unit of work must be done.
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// Constructs a new general unit of work.
        /// </summary>
        protected UnitOfWork() { }

        /// <summary>
        /// Constructs a new general unit of work.
        /// </summary>
        /// <param name="context">Entity Framework DbContext-derived class.</param>
        protected UnitOfWork(DbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the DbContext for the unit of work.
        /// </summary>
        protected DbContext Context { get; set; }

        /// <summary>
        /// Asynchronously saves all changes made to one or more repositories.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects saved.</returns>
        public virtual async Task<int> Save()
        {
            return await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the DbContext.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the DbContext.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
