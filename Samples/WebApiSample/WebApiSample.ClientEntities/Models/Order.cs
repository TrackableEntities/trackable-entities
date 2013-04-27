using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace WebApiSample.ClientEntities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Order : ModelBase<Order>
    {
        public Order()
        {
            this.OrderDetails = new ChangeTrackingCollection<OrderDetail>();
        }

        [DataMember]
        public int OrderId
		{ 
		    get { return _OrderId; }
			set
			{
			    if (value == _OrderId) return;
				_OrderId = value;
				NotifyPropertyChanged(m => m.OrderId);
			}
		}
        private int _OrderId;

        [DataMember]
        public string CustomerId
		{ 
		    get { return _CustomerId; }
			set
			{
			    if (value == _CustomerId) return;
				_CustomerId = value;
				NotifyPropertyChanged(m => m.CustomerId);
			}
		}
        private string _CustomerId;

        [DataMember]
        public Nullable<System.DateTime> OrderDate
		{ 
		    get { return _OrderDate; }
			set
			{
			    if (value == _OrderDate) return;
				_OrderDate = value;
				NotifyPropertyChanged(m => m.OrderDate);
			}
		}
        private Nullable<System.DateTime> _OrderDate;

        [DataMember]
        public Nullable<System.DateTime> ShippedDate
		{ 
		    get { return _ShippedDate; }
			set
			{
			    if (value == _ShippedDate) return;
				_ShippedDate = value;
				NotifyPropertyChanged(m => m.ShippedDate);
			}
		}
        private Nullable<System.DateTime> _ShippedDate;

        [DataMember]
        public Nullable<int> ShipVia
		{ 
		    get { return _ShipVia; }
			set
			{
			    if (value == _ShipVia) return;
				_ShipVia = value;
				NotifyPropertyChanged(m => m.ShipVia);
			}
		}
        private Nullable<int> _ShipVia;

        [DataMember]
        public Nullable<decimal> Freight
		{ 
		    get { return _Freight; }
			set
			{
			    if (value == _Freight) return;
				_Freight = value;
				NotifyPropertyChanged(m => m.Freight);
			}
		}
        private Nullable<decimal> _Freight;

        [DataMember]
        public Customer Customer
		{
		    get { return _Customer; }
			set
			{
			    if (value == _Customer) return;
				_Customer = value;
				NotifyPropertyChanged(m => m.Customer);
			}
		}
        private Customer _Customer;

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

    }
}
