using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SampleWebApi.Inheritance.Entities
{
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class DiscontinuedProduct : Product, 
        IEquatable<DiscontinuedProduct>, INotifyPropertyChanged
    {
		[DataMember]
		public DateTime? DiscontinuedDate
		{ 
			get { return _DiscontinuedDate; }
			set
			{
				if (Equals(value, _DiscontinuedDate)) return;
				_DiscontinuedDate = value;
				if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DiscontinuedDate"));
			}
		}
		private DateTime? _DiscontinuedDate;

        #region Change Tracking

		[JsonProperty, DataMember]
		private Guid EntityIdentifier { get; set; }

		#pragma warning disable 414

		[JsonProperty, DataMember]
		private Guid _entityIdentity = default(Guid);

		#pragma warning restore 414

        bool IEquatable<DiscontinuedProduct>.Equals(DiscontinuedProduct other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}

        #endregion

        #region INotifyPropertyChanged Implementation

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
