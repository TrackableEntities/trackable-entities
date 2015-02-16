using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Client;

namespace WcfSample.Shared.Entities.Net45.Models
{
    [Table("Category")]
    public partial class Category : EntityBase
    {
        public Category()
        {
            Products = new ChangeTrackingCollection<Product>();
        }

		public int CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged();
			}
		}
		private int _CategoryId;

        [Required]
        [StringLength(15)]
		public string CategoryName
		{ 
			get { return _CategoryName; }
			set
			{
				if (Equals(value, _CategoryName)) return;
				_CategoryName = value;
				NotifyPropertyChanged();
			}
		}
		private string _CategoryName;

		public ChangeTrackingCollection<Product> Products
		{
			get { return _Products; }
			set
			{
				if (Equals(value, _Products)) return;
				_Products = value;
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<Product> _Products;
    }
}
