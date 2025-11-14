using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using kOS.MechJeb.Addon.Core;

namespace kOS.MechJeb.Addon.Utils
{
    public static class VesselUtils
    {
        public static IEnumerable<PartModule> FindModuleByTypeName(this Vessel vessel, string moduleTypeName) =>
            vessel.parts.SelectMany(p => p.Modules.Cast<PartModule>()).Where(m =>
                    m.ClassName.Equals(moduleTypeName, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        
        public static object GetComputedModule(object coreInstance, string attrModuleName)
        {
            var getCompModule = coreInstance.GetType().GetMethodInvoker(Constants.GetComputerModuleMethod,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase, typeof(string));
            return getCompModule?.Invoke(coreInstance, new object[] { attrModuleName });
        }
    }
}