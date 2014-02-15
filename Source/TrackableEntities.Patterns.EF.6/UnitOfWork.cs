using System;
using System.Collections;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace TrackableEntities.Patterns.EF6
{
    /// <summary>
    /// General unit of work for committing changes across one or more repositories. 
    /// Inherit from this class to supply a specific DbContext. 
    /// Add a property for each respository for which unit of work must be done.
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork, IUnitOfWorkAsync, IDisposable
    {
        private bool _disposed;

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
            if (_disposed) return;
            if (disposing) Context.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Saves changes made to one or more repositories.
        /// </summary>
        /// <returns>The number of objects saved.</returns>
        public virtual int SaveChanges()
        {
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWork");
            return Context.SaveChanges();
        }

        /// <summary>
        /// Saves changes made to one or more repositories.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects saved.</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWork");
            return await SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Saves changes made to one or more repositories.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects saved.</returns>
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWork");
            return await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
