using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using TrackableEntities.Common;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace TrackableEntities
{
    public static class Cache
    {
        public static Dictionary<Type, List<ExtendedPropertyInfo>> Items = new Dictionary<Type, List<ExtendedPropertyInfo>>();
    }
}