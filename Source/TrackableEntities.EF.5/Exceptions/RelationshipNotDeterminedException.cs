#if EF_6
namespace TrackableEntities.EF6.Exceptions
#else
namespace TrackableEntities.EF5.Exceptions
#endif
{
    /// <summary>
    /// Exception for state when the relationship cannot be determined for a given type and a givven property.
    /// </summary>
    internal class RelationshipNotDeterminedException : TrackableEntitiesException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RelationshipNotDeterminedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error </param>
        public RelationshipNotDeterminedException(string message)
            : base(message)
        {
        }        
    }
}