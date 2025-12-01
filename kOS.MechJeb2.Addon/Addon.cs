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
        private PartModule _mechJebCore;
        private bool _isCoreInitialized = false;
        private bool? _isMechJebDev;

        public Addon(SharedObjects shared) : base(shared)
        {
            RegisterInitializer(InitializeSuffixes);
            TryInitializeMechJebCore();
        }

        public override BooleanValue Available()
        {
            Log.Debug("MJAddon.Available() called");
            if (!_isCoreInitialized) TryInitializeMechJebCore();
            _isMechJebDev ??= _mechJebCore.GetType().Assembly.IsMechJebDevBuild();
            Log.Debug($"MechJeb DEV build detected: {_isMechJebDev}");
            return MechJebController.IsAvailable;
        }

        private void InitializeSuffixes()
        {
            Log.Debug("Initializing kOS suffixes for MJAddon");
            AddSuffix("CORE",
                new NoArgsSuffix<MechJebCoreWrapper>(() =>
                    Available()
                        ? MechJebController.Instance
                        : throw new KOSUnavailableAddonException(
                            "CORE wrapper is unavailable. Please install a MechJeb2 and make sure the MechJebCore module is running on this vessel.",
                            "MechJeb2")));
            AddSuffix("INIT",
                new OneArgsSuffix<BooleanValue>((val) => TryInitializeMechJebCore(val),
                    "Manually (re)initializes the MechJeb core wrapper. Pass TRUE to force reinitialization."));
            AddSuffix("VERSION",
                new NoArgsSuffix<VersionInfo>(GetVersionInfo,
                    "Returns the kOS.MechJeb2.Addon version (major.minor.patch.build)."));
        }

        private VersionInfo GetVersionInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var ver =  asm.GetName().Version;
            return new VersionInfo(ver.Major, ver.Minor, ver.Build, ver.Revision);
        }

        private void TryInitializeMechJebCore(bool force = false)
        {
            Log.Debug($"Trying to initialize MechJebCore (force={force})");
            if (_isCoreInitialized && !force) return;

            var vesselExtensionType = Constants.VesselExtensionName.GetTypeFromCache();
            if (vesselExtensionType == null)
            {
                Log.Error("[kOS.MJ] Cannot find MuMech.VesselExtensions type");
                return;
            }

            var getMasterMechJeb =
                vesselExtensionType.GetMethod("GetMasterMechJeb", BindingFlags.Public | BindingFlags.Static);

            var vessel = shared.Vessel;

            if (vessel == null || getMasterMechJeb == null)
                return;

            try
            {
                var core = getMasterMechJeb?.Invoke(null, new object[] { vessel }) as PartModule;

                if (core == null)
                {
                    Log.Error("Cannot find MechJebCore running module");
                    return;
                }

                _mechJebCore = core;
                Log.Debug("MechJebCore instance initialized successfully");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return;
            }

            MechJebController.Instance.Initialize(_mechJebCore, force);
            _isCoreInitialized = true;
        }
    }
}