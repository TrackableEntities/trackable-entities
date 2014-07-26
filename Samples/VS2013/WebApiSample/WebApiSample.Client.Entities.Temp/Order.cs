namespace WebApiSample.Client.Entities.Temp
{
    using System;
    using System.Collections.Generic;
	using System.Runtime.Serialization;
	using Newtonsoft.Json;
	using TrackableEntities;
	using TrackableEntities.Client;

    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/TrackableEntities.Models")]
    public partial class Order : ModelBase<Order>, IEquatable<Order>, ITrackable
    {
        public Order()
        {
            OrderDetails = new ChangeTrackingCollection<OrderDetail>();
        }

		[DataMember]
		public int OrderId
		{ 
			get { return _OrderId; }
			set
			{
				if (Equals(value, _OrderId)) return;
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
				if (Equals(value, _CustomerId)) return;
				_CustomerId = value;
				NotifyPropertyChanged(m => m.CustomerId);
			}
		}
		private string _CustomerId;

		[DataMember]
		public DateTime? OrderDate
		{ 
			get { return _OrderDate; }
			set
			{
				if (Equals(value, _OrderDate)) return;
				_OrderDate = value;
				NotifyPropertyChanged(m => m.OrderDate);
			}
		}
		private DateTime? _OrderDate;

		[DataMember]
		public DateTime? ShippedDate
		{ 
			get { return _ShippedDate; }
			set
			{
				if (Equals(value, _ShippedDate)) return;
				_ShippedDate = value;
				NotifyPropertyChanged(m => m.ShippedDate);
			}
		}
		private DateTime? _ShippedDate;

		[DataMember]
		public int? ShipVia
		{ 
			get { return _ShipVia; }
			set
			{
				if (Equals(value, _ShipVia)) return;
				_ShipVia = value;
				NotifyPropertyChanged(m => m.ShipVia);
			}
		}
		private int? _ShipVia;

		[DataMember]
		public decimal? Freight
		{ 
			get { return _Freight; }
			set
			{
				if (Equals(value, _Freight)) return;
				_Freight = value;
				NotifyPropertyChanged(m => m.Freight);
			}
		}
		private decimal? _Freight;


		[DataMember]
		public Customer Customer
		{
			get { return _Customer; }
			set
			{
				if (Equals(value, _Customer)) return;
				_Customer = value;
				CustomerChangeTracker = _Customer == null ? null
					: new ChangeTrackingCollection<Customer> { _Customer };
				NotifyPropertyChanged(m => m.Customer);
			}
		}
		private Customer _Customer;
		private ChangeTrackingCollection<Customer> CustomerChangeTracker { get; set; }

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

        #region Change Tracking

		[DataMember]
		public TrackingState TrackingState { get; set; }

		[DataMember]
		public ICollection<string> ModifiedProperties { get; set; }

		[JsonProperty, DataMember]
		private Guid EntityIdentifier { get; set; }

		#pragma warning disable 414

		[JsonProperty, DataMember]
		private Guid _entityIdentity = default(Guid);

		#pragma warning restore 414

		bool IEquatable<Order>.Equals(Order other)
		{
			if (EntityIdentifier != default(Guid))
				return EntityIdentifier == other.EntityIdentifier;
			return false;
		}

        #endregion
    }
}
