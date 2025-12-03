using System;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IBaseWrapper
    {
        void Initialize();

        bool Initialized { get; }
    }
}