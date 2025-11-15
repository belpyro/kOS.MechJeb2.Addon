using System;

namespace kOS.MechJeb2.Addon.Attributes
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