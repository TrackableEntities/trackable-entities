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
    public partial class Product : ModelBase<Product>, ITrackable
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
			    if (value == _ProductId) return;
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
			    if (value == _ProductName) return;
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
			    if (value == _CategoryId) return;
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
			    if (value == _UnitPrice) return;
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
			    if (value == _Discontinued) return;
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
			    if (value == _RowVersion) return;
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
			    if (value == _Category) return;
				_Category = value;
				NotifyPropertyChanged(m => m.Category);
			}
		}
        private Category _Category;

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

        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

        [DataMember]
        public TrackingState TrackingState { get; set; }
    }
}
