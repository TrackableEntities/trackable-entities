using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackableEntities.EF.Tests.NorthwindModels
{
    public partial class Promo
    {
        [Key]
        public int PromoId { get; set; }
        public string PromoCode { get; set; }
    }
}
