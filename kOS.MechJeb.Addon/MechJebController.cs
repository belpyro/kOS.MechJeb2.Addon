using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;

namespace kOS.MechJeb2.Addon
{
    public static class MechJebController
    {
        private static readonly MechJebCoreWrapper _instance = new MechJebCoreWrapper();

        public static MechJebCoreWrapper Instance => _instance;

        public static BooleanValue IsAvailable => Instance != null && Instance.Initialized
                                                                   && Instance.VesselState.Initialized &&
                                                                   Instance.Ascent.Initialized &&
                                                                   Instance.InfoItems.Initialized;

        public static void InitWrapper(object initialObj) => _instance.Initialize(initialObj);
    }
}