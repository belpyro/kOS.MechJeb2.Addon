using System;
using kOS.MechJeb.Addon.Wrapeers;

namespace kOS.MechJeb.Addon.Core
{
    public interface IMechJebCoreWrapper : IBaseWrapper
    {
        MechJebAscentWrapper Ascent { get; }
        VesselStateWrapper  VesselState { get; }
        MechJebInfoItemsWrapper InfoItems { get; }
        Func<object, bool> Running { get; }
    }
}