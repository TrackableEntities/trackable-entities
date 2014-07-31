using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace SampleWebApi.Inheritance.Entities
{
    //[Table("Category")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Category : ModelBase<Category>, IEquatable<Category>, ITrackable
    {
        public Category()
        {
            Products = new ChangeTrackingCollection<Product>();
        }

		[DataMember]
		public int CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged(m => m.CategoryId);
			}
		}
		private int _CategoryId;

        [Required]
        [StringLength(15)]
        [DataMember]
		public string CategoryName
		{ 
			get { return _CategoryName; }
			set
			{
				if (Equals(value, _CategoryName)) return;
				_CategoryName = value;
				NotifyPropertyChanged(m => m.CategoryName);
			}
		}
		private string _CategoryName;

		[DataMember]
		public ChangeTrackingCollection<Product> Products
		{
			get { return _Products; }
			set
			{
				if (Equals(value, _Products)) return;
				_Products = value;
				NotifyPropertyChanged(m => m.Products);
			}
		}
		private ChangeTrackingCollection<Product> _Products;

        #region Change Tracking

        [NotMapped]
        [DataMember]
		public TrackingState TrackingState { get; set; }

        [NotMapped]
        [DataMember]
		public ICollection<string> ModifiedProperties { get; set; }

        [NotMapped]
        [JsonProperty, DataMember]
		private Guid EntityIdentifier { get; set; }

		#pragma warning disable 414

		[JsonProperty, DataMember]
		private Guid _entityIdentity = default(Guid);

		#pragma warning restore 414

		bool IEquatable<Category>.Equals(Category other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}

        #endregion
    }
}
