using System;
using System.Linq;
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
        private static readonly VersionInfo _version = new(0, 0, 1, 0);
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
            return (bool)MechJebController.IsAvailable && (bool)_isMechJebDev;
        }

        private void InitializeSuffixes()
        {
            Log.Debug("Initializing kOS suffixes for MJAddon");
            this.AddSuffix("CORE",
                new NoArgsSuffix<MechJebCoreWrapper>(() =>
                    Available()
                        ? MechJebController.Instance
                        : throw new KOSUnavailableAddonException(
                            "CORE wrapper is unavailable. Please install a MechJeb2 DEV build and make sure the MechJebCore module is running on this vessel.",
                            "MechJeb2")));
            AddSuffix("INIT",
                new OneArgsSuffix<BooleanValue>((val) => TryInitializeMechJebCore(val),
                    "Manually (re)initializes the MechJeb core wrapper. Pass TRUE to force reinitialization."));
            this.AddSuffix("VERSION",
                new NoArgsSuffix<VersionInfo>(() => _version,
                    "Returns the kOS.MechJeb2.Addon version (major.minor.patch.build)."));
        }

        private void TryInitializeMechJebCore(bool force = false)
        {
            Log.Debug($"Trying to initialize MechJebCore (force={force})");
            if (_isCoreInitialized && !force) return;

            var vessel = shared.Vessel;

            if (vessel == null)
                return;

            var mechJebModules =
                vessel?.FindModuleByTypeName(Constants.MechjebCoreModuleName).ToArray();
            
            Log.Debug($"Found {mechJebModules.Length} MechJebCore modules on vessel");
            
            if (mechJebModules.Length == 0)
            {
                Log.Warning("Cannot find MechJebCore modules");
                return;
            }

            var checkIsRunning = Reflect.On(mechJebModules[0])
                .Field("running").AsGetter<bool>();
            if (mechJebModules.Length > 1 && mechJebModules.Count(m => checkIsRunning(m)) > 1)
            {
                Log.Error("Only one MechJebCore module is supported in running state");
                return;
            }

            try
            {
                var core = mechJebModules.SingleOrDefault(m => checkIsRunning(m));

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