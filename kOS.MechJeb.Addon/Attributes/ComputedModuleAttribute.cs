using System;

namespace kOS.MechJeb.Addon.Attributes
{
    public class ComputedModuleAttribute : Attribute
    {
        public string ModuleName { get; set; }

        public ComputedModuleAttribute(string moduleName)
        {
            ModuleName = moduleName;
        }
    }
}