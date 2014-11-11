using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using TrackableEntities.Common;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace TrackableEntities
{
    public class ExtendedPropertyInfo : IEquatable<ExtendedPropertyInfo>
    {
        public ExtendedPropertyInfo(PropertyInfo propertyInfo, Type typeInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.TypeInfo = typeInfo;
        }

        public PropertyInfo PropertyInfo { get; private set; }

        public Type TypeInfo { get; private set; }

        public bool NoUpdate { get; set; }

        public bool ReferenceOnly { get; set; }

        public bool IsManyRelation { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExtendedPropertyInfo);
        }

        public bool Equals(ExtendedPropertyInfo other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;
            else
                return (Object.Equals(this.PropertyInfo, other.PropertyInfo));
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ this.PropertyInfo.GetHashCode();
        }
    }
}
