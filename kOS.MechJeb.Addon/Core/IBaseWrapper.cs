using System;

namespace kOS.MechJeb.Addon.Core
{
    public interface IBaseWrapper
    {
        void Initialize(Object coreInstance);

        bool Initialized { get; }
    }
}