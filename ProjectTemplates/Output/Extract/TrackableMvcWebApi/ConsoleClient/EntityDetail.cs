using System;
using TrackableEntities.Client;

namespace $saferootprojectname$.ClientEntities.Models
{
    public class EntityDetail : ModelBase<EntityDetail>
    {
        public int EntityDetailId { get; set; }
        public int EntityId { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
