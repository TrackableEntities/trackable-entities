using System;
using System.Collections.Generic;
using TrackableEntities.Client;

namespace WcfSample.Client.Entities.Models
{
    public partial class Product : EntityBase
    {
		public Product()
		{
			this.OrderDetails = new ChangeTrackingCollection<OrderDetail>();
		}

		public int ProductId
		{ 
			get { return _ProductId; }
			set
			{
				if (Equals(value, _ProductId)) return;
				_ProductId = value;
				NotifyPropertyChanged();
			}
		}
		private int _ProductId;

		public string ProductName
		{ 
			get { return _ProductName; }
			set
			{
				if (Equals(value, _ProductName)) return;
				_ProductName = value;
				NotifyPropertyChanged();
			}
		}
		private string _ProductName;

		public Nullable<int> CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged();
			}
		}
		private Nullable<int> _CategoryId;

		public Nullable<decimal> UnitPrice
		{ 
			get { return _UnitPrice; }
			set
			{
				if (Equals(value, _UnitPrice)) return;
				_UnitPrice = value;
				NotifyPropertyChanged();
			}
		}
		private Nullable<decimal> _UnitPrice;

		public bool Discontinued
		{ 
			get { return _Discontinued; }
			set
			{
				if (Equals(value, _Discontinued)) return;
				_Discontinued = value;
				NotifyPropertyChanged();
			}
		}
		private bool _Discontinued;

		public byte[] RowVersion
		{ 
			get { return _RowVersion; }
			set
			{
				if (Equals(value, _RowVersion)) return;
				_RowVersion = value;
				NotifyPropertyChanged();
			}
		}
		private byte[] _RowVersion;

		public Category Category
		{
			get { return _Category; }
			set
			{
				if (Equals(value, _Category)) return;
				_Category = value;
				CategoryChangeTracker = _Category == null ? null
					: new ChangeTrackingCollection<Category> { _Category };
				NotifyPropertyChanged();
			}
		}
		private Category _Category;
		private ChangeTrackingCollection<Category> CategoryChangeTracker { get; set; }

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
