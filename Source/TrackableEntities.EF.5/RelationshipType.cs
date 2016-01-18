﻿#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    /// <summary>
    /// Type of relationship between entities.
    /// </summary>
    public enum RelationshipType
    {
        /// <summary>Many to one relationship.</summary>
        ManyToOne,
        /// <summary>One to one relationship.</summary>
        OneToOne,
        /// <summary>Many to many relationship.</summary>
        ManyToMany,
        /// <summary>One to many relationship.</summary>
        OneToMany
    }
}