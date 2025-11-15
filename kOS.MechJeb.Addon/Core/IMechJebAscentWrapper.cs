using System;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IMechJebAscentWrapper : IBaseWrapper
    {
        Func<object,bool> Enabled { get; set; }
    }
}