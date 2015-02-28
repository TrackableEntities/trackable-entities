using System;
using System.Collections.Generic;
using TrackableEntities.Client;

namespace WebApiSample.Client.Entities.Models
{
    public partial class Order : EntityBase
    {
		public Order()
		{
			this.OrderDetails = new ChangeTrackingCollection<OrderDetail>();
		}

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

		public string CustomerId
		{ 
			get { return _CustomerId; }
			set
			{
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged(() => CustomerId);
			}
		}
		private string _CustomerId;

		public Nullable<System.DateTime> OrderDate
		{ 
			get { return _OrderDate; }
			set
			{
				if (Equals(value, _OrderDate)) return;
				_OrderDate = value;
				NotifyPropertyChanged(() => OrderDate);
			}
		}
		private Nullable<System.DateTime> _OrderDate;

		public Nullable<System.DateTime> ShippedDate
		{ 
			get { return _ShippedDate; }
			set
			{
				if (Equals(value, _ShippedDate)) return;
				_ShippedDate = value;
				NotifyPropertyChanged(() => ShippedDate);
			}
		}
		private Nullable<System.DateTime> _ShippedDate;

		public Nullable<int> ShipVia
		{ 
			get { return _ShipVia; }
			set
			{
				if (Equals(value, _ShipVia)) return;
				_ShipVia = value;
				NotifyPropertyChanged(() => ShipVia);
			}
		}
		private Nullable<int> _ShipVia;

		public Nullable<decimal> Freight
		{ 
			get { return _Freight; }
			set
			{
				if (Equals(value, _Freight)) return;
				_Freight = value;
				NotifyPropertyChanged(() => Freight);
			}
		}
		private Nullable<decimal> _Freight;

		public Customer Customer
		{
			get { return _Customer; }
			set
			{
				if (Equals(value, _Customer)) return;
				_Customer = value;
				CustomerChangeTracker = _Customer == null ? null
					: new ChangeTrackingCollection<Customer> { _Customer };
				NotifyPropertyChanged(() => Customer);
			}
		}
		private Customer _Customer;
		private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }

		public ChangeTrackingCollection<OrderDetail> OrderDetails
		{
			get { return _OrderDetails; }
			set
			{
				if (Equals(value, _OrderDetails)) return;
				_OrderDetails = value;
				NotifyPropertyChanged(() => OrderDetails);
			}
		}
		private ChangeTrackingCollection<OrderDetail> _OrderDetails;

	}
}
