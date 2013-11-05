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
    public partial class Customer : ModelBase<Customer>, ITrackable
    {
        public Customer()
        {
            this.Orders = new ChangeTrackingCollection<Order>();
        }

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
        public string CompanyName
		{ 
		    get { return _CompanyName; }
			set
			{
			    if (value == _CompanyName) return;
				_CompanyName = value;
				NotifyPropertyChanged(m => m.CompanyName);
			}
		}
        private string _CompanyName;

        [DataMember]
        public string ContactName
		{ 
		    get { return _ContactName; }
			set
			{
			    if (value == _ContactName) return;
				_ContactName = value;
				NotifyPropertyChanged(m => m.ContactName);
			}
		}
        private string _ContactName;

        [DataMember]
        public string City
		{ 
		    get { return _City; }
			set
			{
			    if (value == _City) return;
				_City = value;
				NotifyPropertyChanged(m => m.City);
			}
		}
        private string _City;

        [DataMember]
        public string Country
		{ 
		    get { return _Country; }
			set
			{
			    if (value == _Country) return;
				_Country = value;
				NotifyPropertyChanged(m => m.Country);
			}
		}
        private string _Country;

        [DataMember]
        public ChangeTrackingCollection<Order> Orders
		{
		    get { return _Orders; }
			set
			{
			    if (Equals(value, _Orders)) return;
				_Orders = value;
				NotifyPropertyChanged(m => m.Orders);
			}
		}
        private ChangeTrackingCollection<Order> _Orders;

        [DataMember]
        public ICollection<string> ModifiedProperties { get; set; }

        [DataMember]
        public TrackingState TrackingState { get; set; }
    }
}
