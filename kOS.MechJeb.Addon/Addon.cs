using System.Linq;
using System.Reflection;
using kOS.AddOns;
using kOS.MechJeb.Addon.Core;
using kOS.MechJeb.Addon.Utils;
using kOS.MechJeb.Addon.Wrapeers;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

// ReSharper disable PossibleNullReferenceException

namespace kOS.MechJeb.Addon
{
    [kOSAddon("MJ")]
    [KOSNomenclature("MJAddon")]
    public class Addon : Suffixed.Addon
    {
        private readonly PartModule _mechJebCore;
        private bool _isAvailable;

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
        }
    }
}