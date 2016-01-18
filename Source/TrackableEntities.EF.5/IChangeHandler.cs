using System;
using System.Collections;
using System.Collections.Generic;
#if EF_6
using System.Data.Entity;
#else
using System.Data;
using System.Data.Entity;
#endif

#if EF_6
namespace TrackableEntities.EF6
#else
namespace TrackableEntities.EF5
#endif
{
    public interface IChangeHandler
    {
        DbContext DbContext { get; set; }
        IList<Delegate> Handlers { get; set; }
    }
}