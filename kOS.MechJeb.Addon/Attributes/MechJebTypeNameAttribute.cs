using System;

namespace kOS.MechJeb2.Addon.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property,  AllowMultiple = false, Inherited = true)]
    public class MechJebTypeNameAttribute : Attribute
    {
        public MechJebTypeNameAttribute(string mechJebTypeName, Type wrapperType)
        {
            this.MechJebTypeName = mechJebTypeName;
            this.WrapperType = wrapperType;
        }

        public Type WrapperType { get; set; }

        public string MechJebTypeName   { get; set; }
    }
}