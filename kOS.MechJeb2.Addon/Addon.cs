using System.Linq;
using kOS.AddOns;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

// ReSharper disable PossibleNullReferenceException

namespace kOS.MechJeb2.Addon
{
    [kOSAddon("MJ")]
    [KOSNomenclature("MJAddon")]
    public class Addon : Suffixed.Addon
    {
        private readonly PartModule _mechJebCore;
        private bool _isAvailable;
        private static readonly VersionInfo _version = new VersionInfo(0, 0, 1, 0);

        public Addon(SharedObjects shared) : base(shared)
        {
            _mechJebCore = shared.Vessel.FindModuleByTypeName(Constants.MechjebCoreModuleName).FirstOrDefault();
            _isAvailable = _mechJebCore != null;
            if (!_isAvailable) return;
            MechJebController.InitWrapper(this._mechJebCore);
            RegisterInitializer(InitializeSuffixes);
        }

        public override BooleanValue Available() => MechJebController.IsAvailable;

        private void InitializeSuffixes()
        {
            this.AddSuffix("CORE", new NoArgsSuffix<MechJebCoreWrapper>(() => MechJebController.Instance));
            this.AddSuffix("VERSION", new NoArgsSuffix<VersionInfo>(() => _version));
        }
    }
}