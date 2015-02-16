using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Client;

namespace WcfSample.Shared.Entities.Net45.Models
{
    [Table("Order")]
    public partial class Order : EntityBase
    {
        public Order()
        {
            OrderDetails = new ChangeTrackingCollection<OrderDetail>();
        }

		public int OrderId
		{ 
			get { return _OrderId; }
			set
			{
				if (Equals(value, _OrderId)) return;
				_OrderId = value;
				NotifyPropertyChanged();
			}
		}
		private int _OrderId;

        [StringLength(5)]
		public string CustomerId
		{ 
			get { return _CustomerId; }
			set
			{
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged();
			}
		}
		private string _CustomerId;

		public DateTime? OrderDate
		{ 
			get { return _OrderDate; }
			set
			{
				if (Equals(value, _OrderDate)) return;
				_OrderDate = value;
				NotifyPropertyChanged();
			}
		}
		private DateTime? _OrderDate;

		public DateTime? ShippedDate
		{ 
			get { return _ShippedDate; }
			set
			{
				if (Equals(value, _ShippedDate)) return;
				_ShippedDate = value;
				NotifyPropertyChanged();
			}
		}
		private DateTime? _ShippedDate;

		public int? ShipVia
		{ 
			get { return _ShipVia; }
			set
			{
				if (Equals(value, _ShipVia)) return;
				_ShipVia = value;
				NotifyPropertyChanged();
			}
		}
		private int? _ShipVia;

        [Column(TypeName = "money")]
		public decimal? Freight
		{ 
			get { return _Freight; }
			set
			{
				if (Equals(value, _Freight)) return;
				_Freight = value;
				NotifyPropertyChanged();
			}
		}
		private decimal? _Freight;


		public Customer Customer
		{
			get { return _Customer; }
			set
			{
				if (Equals(value, _Customer)) return;
				_Customer = value;
				CustomerChangeTracker = _Customer == null ? null
					: new ChangeTrackingCollection<Customer> { _Customer };
				NotifyPropertyChanged();
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
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<OrderDetail> _OrderDetails;
    }
}
