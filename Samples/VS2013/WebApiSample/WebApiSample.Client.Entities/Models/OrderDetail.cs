using System;
using System.Collections.Generic;
using TrackableEntities.Client;

namespace WebApiSample.Client.Entities.Models
{
    public partial class OrderDetail : EntityBase
    {
		public int OrderDetailId
		{ 
			get { return _OrderDetailId; }
			set
			{
				if (Equals(value, _OrderDetailId)) return;
				_OrderDetailId = value;
				NotifyPropertyChanged(() => OrderDetailId);
			}
		}
		private int _OrderDetailId;

		public int OrderId
		{ 
			get { return _OrderId; }
			set
			{
				if (Equals(value, _OrderId)) return;
				_OrderId = value;
				NotifyPropertyChanged(() => OrderId);
			}
		}
		private int _OrderId;

		public int ProductId
		{ 
			get { return _ProductId; }
			set
			{
				if (Equals(value, _ProductId)) return;
				_ProductId = value;
				NotifyPropertyChanged(() => ProductId);
			}
		}
		private int _ProductId;

		public decimal UnitPrice
		{ 
			get { return _UnitPrice; }
			set
			{
				if (Equals(value, _UnitPrice)) return;
				_UnitPrice = value;
				NotifyPropertyChanged(() => UnitPrice);
			}
		}
		private decimal _UnitPrice;

		public short Quantity
		{ 
			get { return _Quantity; }
			set
			{
				if (Equals(value, _Quantity)) return;
				_Quantity = value;
				NotifyPropertyChanged(() => Quantity);
			}
		}
		private short _Quantity;

		public float Discount
		{ 
			get { return _Discount; }
			set
			{
				if (Equals(value, _Discount)) return;
				_Discount = value;
				NotifyPropertyChanged(() => Discount);
			}
		}
		private float _Discount;

		public Order Order
		{
			get { return _Order; }
			set
			{
				if (Equals(value, _Order)) return;
				_Order = value;
				OrderChangeTracker = _Order == null ? null
					: new ChangeTrackingCollection<Order> { _Order };
				NotifyPropertyChanged(() => Order);
			}
		}
		private Order _Order;
		private ChangeTrackingCollection<Order> OrderChangeTracker { get; set; }

		public Product Product
		{
			get { return _Product; }
			set
			{
				if (Equals(value, _Product)) return;
				_Product = value;
				ProductChangeTracker = _Product == null ? null
					: new ChangeTrackingCollection<Product> { _Product };
				NotifyPropertyChanged(() => Product);
			}
		}
		private Product _Product;
		private ChangeTrackingCollection<Product> ProductChangeTracker { get; set; }

	}
}
