using System;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IBaseWrapper
    {
        void Initialize();

        /// <summary>
        /// Reinitialize the wrapper, clearing any cached references.
        /// Used after save reloads or vessel changes when MechJeb references become stale.
        /// </summary>
        void Reinitialize();

        bool Initialized { get; }
    }
}
