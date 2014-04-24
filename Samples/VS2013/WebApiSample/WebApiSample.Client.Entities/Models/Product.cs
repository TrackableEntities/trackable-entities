using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace WebApiSample.Client.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Product : ModelBase<Product>, IEquatable<Product>, ITrackable
    {
		public Product()
		{
			this.OrderDetails = new ChangeTrackingCollection<OrderDetail>();
		}

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
		public Nullable<int> CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged(m => m.CategoryId);
			}
		}
		private Nullable<int> _CategoryId;

		[DataMember]
		public Nullable<decimal> UnitPrice
		{ 
			get { return _UnitPrice; }
			set
			{
				if (Equals(value, _UnitPrice)) return;
				_UnitPrice = value;
				NotifyPropertyChanged(m => m.UnitPrice);
			}
		}
		private Nullable<decimal> _UnitPrice;

		[DataMember]
		public bool Discontinued
		{ 
			get { return _Discontinued; }
			set
			{
				if (Equals(value, _Discontinued)) return;
				_Discontinued = value;
				NotifyPropertyChanged(m => m.Discontinued);
			}
		}
		private bool _Discontinued;

		[DataMember]
		public byte[] RowVersion
		{ 
			get { return _RowVersion; }
			set
			{
				if (Equals(value, _RowVersion)) return;
				_RowVersion = value;
				NotifyPropertyChanged(m => m.RowVersion);
			}
		}
		private byte[] _RowVersion;

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

		[DataMember]
		public ChangeTrackingCollection<OrderDetail> OrderDetails
		{
			get { return _OrderDetails; }
			set
			{
				if (Equals(value, _OrderDetails)) return;
				_OrderDetails = value;
				NotifyPropertyChanged(m => m.OrderDetails);
			}
		}
		private ChangeTrackingCollection<OrderDetail> _OrderDetails;

        #region Change Tracking

		[DataMember]
		public TrackingState TrackingState { get; set; }

		[DataMember]
		public ICollection<string> ModifiedProperties { get; set; }

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
