using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackableEntities.Client.Tests.Entities.MultiParentModels
{
	public class ParentType1 : EntityBase
	{
		public ParentType1()
		{
			Children = new ChangeTrackingCollection<MultiParentChild>();
		}

		public ChangeTrackingCollection<MultiParentChild> Children { get; set; }
	}
}
