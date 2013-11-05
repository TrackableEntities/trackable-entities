using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities;
using TrackableEntities.Client;

namespace WcfSample.Client.Entities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class OrderDetail : ModelBase<OrderDetail>, ITrackable
    {
        [DataMember]
        public int OrderDetailId
		{ 
		    get { return _OrderDetailId; }
			set
			{
			    if (value == _OrderDetailId) return;
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
			    if (value == _OrderId) return;
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
			    if (value == _ProductId) return;
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
			    if (value == _UnitPrice) return;
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
			    if (value == _Quantity) return;
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
			    if (value == _Discount) return;
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
			    if (value == _Order) return;
				_Order = value;
				NotifyPropertyChanged(m => m.Order);
			}
		}
        private Order _Order;

        [DataMember]
        public Product Product
		{
		    get { return _Product; }
			set
			{
			    if (value == _Product) return;
				_Product = value;
				NotifyPropertyChanged(m => m.Product);
			}
		}
        private Product _Product;

        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

        [DataMember]
        public TrackingState TrackingState { get; set; }
    }
}
