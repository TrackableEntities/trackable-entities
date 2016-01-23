using System;

#if EF_6
namespace TrackableEntities.EF6.Exceptions
#else
namespace TrackableEntities.EF5.Exceptions
#endif
{
    /// <summary>
    /// Base class for exceptions defined in Trackable Entities.
    /// </summary>
    public abstract class TrackableEntitiesException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrackableEntitiesException"/> class.
        /// </summary>
        protected TrackableEntitiesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrackableEntitiesException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        protected TrackableEntitiesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrackableEntitiesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        protected TrackableEntitiesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
