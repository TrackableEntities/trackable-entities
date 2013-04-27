using System;
using TrackableEntities.Client;

namespace $saferootprojectname$.ClientEntities.Models
{
    public class Entity : ModelBase<Entity>
    {
        public int EntityId { get; set; }
        public ChangeTrackingCollection<EntityDetail> EntityDetails { get; set; }
    }
}
