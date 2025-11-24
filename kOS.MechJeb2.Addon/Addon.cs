using System.Linq;
using kOS.AddOns;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;

// ReSharper disable PossibleNullReferenceException

namespace kOS.MechJeb2.Addon
{
    [kOSAddon("MJ")]
    [KOSNomenclature("MJAddon")]
    public class Addon : Suffixed.Addon
    {
        private PartModule _mechJebCore;
        private bool _isCoreInitialized = false;
        private static readonly VersionInfo _version = new(0, 0, 1, 0);
        private bool? _isMechJebDev;

        public Addon(SharedObjects shared) : base(shared)
        {
            RegisterInitializer(InitializeSuffixes);
            TryInitializeMechJebCore();
        }

        public override BooleanValue Available()
        {
            if (!_isCoreInitialized) TryInitializeMechJebCore();
            _isMechJebDev ??= _mechJebCore.GetType().Assembly.IsMechJebDevBuild();
            return (bool)MechJebController.IsAvailable && (bool)_isMechJebDev;
        }

        private void InitializeSuffixes()
        {
            this.AddSuffix("CORE",
                new NoArgsSuffix<MechJebCoreWrapper>(() =>
                    Available()
                        ? MechJebController.Instance
                        : throw new KOSUnavailableAddonException("CORE is unavailable. Please, make sure that you use MechJeb2 DEV build", "MechJeb2")));
            this.AddSuffix("VERSION", new NoArgsSuffix<VersionInfo>(() => _version));
        }

        private void TryInitializeMechJebCore()
        {
            if (_isCoreInitialized) return;

            var vessel = shared.Vessel;
            var part = shared.KSPPart;

            if (vessel == null && part == null)
                return;

            var core =
                vessel?.FindModuleByTypeName(Constants.MechjebCoreModuleName).FirstOrDefault()
                ?? part?.Modules.GetModule(Constants.MechjebCoreModuleName);

            if (core == null)
                return; // not loaded vessel

            _mechJebCore = core;
            MechJebController.InitWrapper(_mechJebCore);
            _isCoreInitialized = true;
        }
    }
}