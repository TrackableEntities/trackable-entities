using System;
using System.Threading.Tasks;

namespace TrackableEntities.Patterns
{
    /// <summary>
    /// Unit of work for committing changes across one or more repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Asynchronously saves all changes made to one or more repositories.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects saved.</returns>
        Task<int> Save();
    }
}
