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
        /// Saves changes made to one or more repositories.
        /// </summary>
        /// <returns>The number of objects saved.</returns>
        int SaveChanges();
    }
}
