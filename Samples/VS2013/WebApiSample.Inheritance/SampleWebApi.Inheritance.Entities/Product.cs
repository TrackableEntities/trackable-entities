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
    //[Table("Product")]
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Product : ModelBase<Product>, IEquatable<Product>, ITrackable
    {
		[DataMember]
		public int ProductId
		{ 
			get { return _ProductId; }
			set
			{
				if (Equals(value, _ProductId)) return;
				_ProductId = value;
				NotifyPropertyChanged(m => m.ProductId);
			}
		}
		private int _ProductId;

        [Required]
        [StringLength(40)]
        [DataMember]
		public string ProductName
		{ 
			get { return _ProductName; }
			set
			{
				if (Equals(value, _ProductName)) return;
				_ProductName = value;
				NotifyPropertyChanged(m => m.ProductName);
			}
		}
		private string _ProductName;

		[DataMember]
		public int? CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged(m => m.CategoryId);
			}
		}
		private int? _CategoryId;

        [Column(TypeName = "money")]
        [DataMember]
		public decimal? UnitPrice
		{ 
			get { return _UnitPrice; }
			set
			{
				if (Equals(value, _UnitPrice)) return;
				_UnitPrice = value;
				NotifyPropertyChanged(m => m.UnitPrice);
			}
		}
		private decimal? _UnitPrice;

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        [DataMember]
        public byte[] RowVersion { get; set; }

		[DataMember]
		public Category Category
		{
			get { return _Category; }
			set
			{
				if (Equals(value, _Category)) return;
				_Category = value;
				CategoryChangeTracker = _Category == null ? null
					: new ChangeTrackingCollection<Category> { _Category };
				NotifyPropertyChanged(m => m.Category);
			}
		}
		private Category _Category;
		private ChangeTrackingCollection<Category> CategoryChangeTracker { get; set; }

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

		bool IEquatable<Product>.Equals(Product other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}

        #endregion
    }
}
