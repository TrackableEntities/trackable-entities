using System;

namespace SampleWebApi.Inheritance.Shared.Entities.Models
{
    public partial class DiscontinuedProduct : Product
    {
		public DateTime? DiscontinuedDate
		{ 
			get { return _DiscontinuedDate; }
			set
			{
                if (Equals(value, _DiscontinuedDate)) return;
                _DiscontinuedDate = value;
                NotifyPropertyChanged();
			}
		}
		private DateTime? _DiscontinuedDate;
    }
}
