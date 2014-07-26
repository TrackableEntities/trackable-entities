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
    public partial class OrderDetail : ModelBase<OrderDetail>, IEquatable<OrderDetail>, ITrackable
    {
		[DataMember]
		public int OrderDetailId
		{ 
			get { return _OrderDetailId; }
			set
			{
				if (Equals(value, _OrderDetailId)) return;
				_OrderDetailId = value;
				NotifyPropertyChanged(m => m.OrderDetailId);
			}
		}
		private int _OrderDetailId;

		[DataMember]
		public int OrderId
		{ 
			get { return _OrderId; }
			set
			{
				if (Equals(value, _OrderId)) return;
				_OrderId = value;
				NotifyPropertyChanged(m => m.OrderId);
			}
		}
		private int _OrderId;

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
		public decimal UnitPrice
		{ 
			get { return _UnitPrice; }
			set
			{
				if (Equals(value, _UnitPrice)) return;
				_UnitPrice = value;
				NotifyPropertyChanged(m => m.UnitPrice);
			}
		}
		private decimal _UnitPrice;

		[DataMember]
		public short Quantity
		{ 
			get { return _Quantity; }
			set
			{
				if (Equals(value, _Quantity)) return;
				_Quantity = value;
				NotifyPropertyChanged(m => m.Quantity);
			}
		}
		private short _Quantity;

		[DataMember]
		public float Discount
		{ 
			get { return _Discount; }
			set
			{
				if (Equals(value, _Discount)) return;
				_Discount = value;
				NotifyPropertyChanged(m => m.Discount);
			}
		}
		private float _Discount;


		[DataMember]
		public Order Order
		{
			get { return _Order; }
			set
			{
				if (Equals(value, _Order)) return;
				_Order = value;
				OrderChangeTracker = _Order == null ? null
					: new ChangeTrackingCollection<Order> { _Order };
				NotifyPropertyChanged(m => m.Order);
			}
		}
		private Order _Order;
		private ChangeTrackingCollection<Order> OrderChangeTracker { get; set; }


		[DataMember]
		public Product Product
		{
			get { return _Product; }
			set
			{
				if (Equals(value, _Product)) return;
				_Product = value;
				ProductChangeTracker = _Product == null ? null
					: new ChangeTrackingCollection<Product> { _Product };
				NotifyPropertyChanged(m => m.Product);
			}
		}
		private Product _Product;
		private ChangeTrackingCollection<Product> ProductChangeTracker { get; set; }

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

		bool IEquatable<OrderDetail>.Equals(OrderDetail other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}

        #endregion
    }
}
