using System;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IBaseWrapper
    {
        void Initialize(Object coreInstance);

        bool Initialized { get; }
    }
}