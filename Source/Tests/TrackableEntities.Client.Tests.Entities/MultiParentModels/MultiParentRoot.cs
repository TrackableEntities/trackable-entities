using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackableEntities.Client.Tests.Entities.MultiParentModels
{
	public class MultiParentRoot : EntityBase
	{
		public MultiParentRoot()
		{
			Parent1Items = new ChangeTrackingCollection<ParentType1>();
			Parent2Items = new ChangeTrackingCollection<ParentType2>();
		}

		public ChangeTrackingCollection<ParentType1> Parent1Items { get; set; }
		public ChangeTrackingCollection<ParentType2> Parent2Items { get; set; }
	}
}
