using System;

namespace kOS.MechJeb.Addon.Core
{
    public interface IMechJebAscentWrapper : IBaseWrapper
    {
        Func<object,bool> Enabled { get; set; }
    }
}