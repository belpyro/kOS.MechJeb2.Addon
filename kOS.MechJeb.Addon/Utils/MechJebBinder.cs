using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using kOS.MechJeb.Addon.Attributes;
using kOS.MechJeb.Addon.Core;

namespace kOS.MechJeb.Addon.Utils
{
    public static class MechJebBinder
    {
        private static readonly BindingFlags CoreFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        // кэш (оптимизация, чтобы не гонять GetProperties/GetField каждый раз)
        private static readonly Dictionary<(Type wrapperType, Type coreType), PropertyInfo[]> _ifacePropsCache
            = new Dictionary<(Type, Type), PropertyInfo[]>();

        public static void BindCoreWrapper(this IBaseWrapper wrapper, object coreInstance)
        {
            var wrapperType = wrapper.GetType();
            var coreType = coreInstance.GetType();
            var key = (wrapperType, coreType);

            if (!_ifacePropsCache.TryGetValue(key, out var ifaceProps))
            {
                ifaceProps = wrapperType.GetProperties();
                _ifacePropsCache[key] = ifaceProps;
            }

            foreach (var ifaceProp in ifaceProps)
            {
                var wrapperProp = wrapperType.GetProperty(ifaceProp.Name, CoreFlags);
                if (wrapperProp == null)
                    continue;

                MemberInfo coreMember =
                    (MemberInfo)coreType.GetProperty(ifaceProp.Name, CoreFlags) ??
                    coreType.GetField(ifaceProp.Name, CoreFlags);

                if (coreMember == null &&
                    wrapperProp.CustomAttributes.All(a => a.AttributeType != typeof(ComputedModuleAttribute)))
                    continue;

                var wrapperPropType = wrapperProp.PropertyType;

                // 1) Примитивы / делегаты
                if (typeof(Delegate).IsAssignableFrom(wrapperPropType))
                {
                    var del = coreMember.BuildTypedGetter(); // Func<object,bool>/double/... как у тебя
                    wrapperProp.SetValue(wrapper, del, null);
                    continue;
                }

                // 2) Вложенный враппер
                if (typeof(IBaseWrapper).IsAssignableFrom(wrapperPropType))
                {
                    var nested = wrapperProp.GetValue(wrapper, null) as IBaseWrapper;
                    if (nested == null) continue;

                    var attr = wrapperProp.GetCustomAttribute<ComputedModuleAttribute>();
                    var memberInstance = attr != null
                        ? VesselUtils.GetComputedModule(coreInstance, attr.ModuleName)
                        : GetMemberValue(coreInstance, coreMember);

                    if (memberInstance == null) continue;
                    nested.Initialize(memberInstance);
                }
            }
        }

        private static Type GetMemberType(MemberInfo member)
        {
            if (member is PropertyInfo pi) return pi.PropertyType;
            if (member is FieldInfo fi) return fi.FieldType;
            throw new NotSupportedException("Only fields and properties are supported");
        }

        private static object GetMemberValue(object obj, MemberInfo member)
        {
            if (member is PropertyInfo pi) return pi.GetValue(obj, null);
            if (member is FieldInfo fi) return fi.GetValue(obj);
            throw new NotSupportedException("Only fields and properties are supported");
        }
    }
}