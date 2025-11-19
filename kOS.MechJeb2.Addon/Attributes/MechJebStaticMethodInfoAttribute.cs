using System;

namespace kOS.MechJeb2.Addon.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    internal class MechJebStaticMethodInfoAttribute : Attribute
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public MechJebStaticMethodInfoAttribute(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}