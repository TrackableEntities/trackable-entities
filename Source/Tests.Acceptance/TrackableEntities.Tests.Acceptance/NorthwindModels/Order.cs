using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    [JsonObject(IsReference = true)]
    public class Order : ITrackable
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        [Column]
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
