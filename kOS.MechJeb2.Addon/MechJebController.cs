using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;

namespace kOS.MechJeb2.Addon
{
    public static class MechJebController
    {
        private static MechJebCoreWrapper _instance;

        public static MechJebCoreWrapper Instance => _instance ??= new MechJebCoreWrapper();

        public static BooleanValue IsAvailable => Instance is { Initialized: true };
    }
}