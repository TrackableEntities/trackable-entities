using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities.Client;

namespace WebApiSample.ClientEntities.Models
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Category : ModelBase<Category>
    {
        public Category()
        {
            this.Products = new ChangeTrackingCollection<Product>();
        }

        [DataMember]
        public int CategoryId
		{ 
		    get { return _CategoryId; }
			set
			{
			    if (value == _CategoryId) return;
				_CategoryId = value;
				NotifyPropertyChanged(m => m.CategoryId);
			}
		}
        private int _CategoryId;

        [DataMember]
        public string CategoryName
		{ 
		    get { return _CategoryName; }
			set
			{
			    if (value == _CategoryName) return;
				_CategoryName = value;
				NotifyPropertyChanged(m => m.CategoryName);
			}
		}
        private string _CategoryName;

        [DataMember]
        public ChangeTrackingCollection<Product> Products
		{
		    get { return _Products; }
			set
			{
			    if (Equals(value, _Products)) return;
				_Products = value;
				NotifyPropertyChanged(m => m.Products);
			}
		}
        private ChangeTrackingCollection<Product> _Products;

    }
}
