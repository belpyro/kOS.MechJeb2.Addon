using System;
using kOS.MechJeb2.Addon.Wrapeers;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IMechJebCoreWrapper : IBaseWrapper
    {
        MechJebAscentWrapper Ascent { get; }
        VesselStateWrapper  VesselState { get; }
        MechJebInfoItemsWrapper InfoItems { get; }
        MechJebLandingAutopilotWrapper Landing { get; }
        Func<object, bool> Running { get; }
    }
}