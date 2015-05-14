using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrackableEntities.Client
{
    /// <summary>
    /// Reflection helper singleton class.
    ///
    /// Lists all functions (defines the contract) which must be implemented by the Impl-class,
    /// and ensures that all Impl-classes implement the identical interface.
    /// 
    /// The actual Impl-class is chosen depending on the conditional compiler #define's.
    /// 
    /// Should there become a need in the future for yet another varying reflection code,
    /// it is highly recommended to extend this class (together with Impl-subclasses) instead
    /// of inlining the #if-#else-#endif logic in the regular TE code.
    /// </summary>
    internal abstract class PortableReflectionHelper
    {
        #region Single Instance

        private static Lazy<PortableReflectionHelper> _inst =
            new Lazy<PortableReflectionHelper>(() => new PortableReflectionHelperImpl(), true);

        public static PortableReflectionHelper Instance
        {
            get { return _inst.Value; }
        }

        #endregion Single Instance

        public abstract bool IsAssignable(Type type, Type from);

        public abstract Type GetBaseType(Type type);

        public abstract Type[] GetGenericArguments(Type type);

        public abstract PropertyInfo GetProperty(Type type, string propertyName);

        public abstract IEnumerable<PropertyInfo> GetProperties(Type type);

        public abstract IEnumerable<PropertyInfo> GetPrivateInstanceProperties(Type type);

        public abstract IEnumerable<MethodInfo> GetPrivateInstanceMethods(Type type);

        public abstract MethodInfo GetMethodInfo(Delegate del);
    }

#if SILVERLIGHT || NET40

    internal class PortableReflectionHelperImpl : PortableReflectionHelper
    {
        public override bool IsAssignable(Type type, Type from)
        {
            return type.IsAssignableFrom(from);
        }

        public override Type GetBaseType(Type type)
        {
            return type.BaseType;
        }

        public override Type[] GetGenericArguments(Type type)
        {
            return type.GetGenericArguments();
        }

        public override PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName);
        }

        public override IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties();
        }

        public override IEnumerable<PropertyInfo> GetPrivateInstanceProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override IEnumerable<MethodInfo> GetPrivateInstanceMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override MethodInfo GetMethodInfo(Delegate del)
        {
            return del.Method;
        }
    }

#else

    internal class PortableReflectionHelperImpl : PortableReflectionHelper
    {
        public override bool IsAssignable(Type type, Type from)
        {
            return type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
        }

        public override Type GetBaseType(Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public override Type[] GetGenericArguments(Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public override PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetTypeInfo().GetDeclaredProperty(propertyName);
        }

        public override IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type
                .BaseTypes()
                .SelectMany(t => t.GetTypeInfo().DeclaredProperties)
                .Where(p => !p.GetMethod.IsPrivate);
        }

        public override IEnumerable<PropertyInfo> GetPrivateInstanceProperties(Type type)
        {
            return type
                .GetTypeInfo()
                .DeclaredProperties
                .Where(p => !p.GetMethod.IsStatic && p.GetMethod.IsPrivate);
        }

        public override IEnumerable<MethodInfo> GetPrivateInstanceMethods(Type type)
        {
            return type
                .GetTypeInfo()
                .DeclaredMethods
                .Where(m => !m.IsStatic && m.IsPrivate);
        }

        public override MethodInfo GetMethodInfo(Delegate del)
        {
            return del.GetMethodInfo();
        }
    }

#endif
}
