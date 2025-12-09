using System;
using System.Linq;
using System.Reflection;
using kOS.AddOns;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using KSPBuildTools;

// ReSharper disable PossibleNullReferenceException

namespace kOS.MechJeb2.Addon
{
    [kOSAddon("MJ")]
    [KOSNomenclature("MJAddon")]
    public class Addon : Suffixed.Addon
    {
        public Addon(SharedObjects shared) : base(shared)
        {
            RegisterInitializer(InitializeSuffixes);
        }

        public override BooleanValue Available()
        {
            Log.Debug("MJAddon.Available() called");

            // Check if GameEvents triggered reinitialization (save reload, vessel change, scene change)
            if (MechJebController.NeedsReinitialization)
            {
                Log.Debug("NeedsReinitialization flag set - forcing reinitialization");
                MechJebController.Instance.ForceReinitialize();
            }

            return MechJebController.Instance.IsAvailable;
        }

        private void InitializeSuffixes()
        {
            Log.Debug("Initializing kOS suffixes for MJAddon");
            AddSuffix("CORE",
                new NoArgsSuffix<MechJebCoreWrapper>(() =>
                    MechJebController.Instance.Core
                ));
            AddSuffix(new[] { "VESSEL", "VESSELINFO" },
                new NoArgsSuffix<VesselStateWrapper>(() => MechJebController.Instance.VesselState
                ));
            AddSuffix(new[] { "ASCENT", "ASCENTGUIDANCE" }, new NoArgsSuffix<MechJebAscentWrapper>(() =>
                MechJebController.Instance.AscentWrapper
            ));
            AddSuffix(new[] { "INFO" }, new NoArgsSuffix<MechJebInfoItemsWrapper>(() =>
                MechJebController.Instance.InfoItems
            ));
            AddSuffix("VERSION",
                new NoArgsSuffix<VersionInfo>(GetVersionInfo,
                    "Returns the kOS.MechJeb2.Addon version (major.minor.patch.build)."));
        }

        private VersionInfo GetVersionInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            return new VersionInfo(ver.Major, ver.Minor, ver.Build, ver.Revision);
        }
    }
}
