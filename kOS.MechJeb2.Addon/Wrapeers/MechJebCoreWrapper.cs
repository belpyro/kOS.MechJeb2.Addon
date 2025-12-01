using System;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("CoreWrapper")]
    public class MechJebCoreWrapper : BaseWrapper, IMechJebCoreWrapper
    {
        private MechJebAscentWrapper _ascentWrapper;
        private VesselStateWrapper _vesselStateWrapper;
        private MechJebInfoItemsWrapper _infoItemsWrapper;
        private MechJebLandingAutopilotWrapper _landingWrapper;

        // Override MasterMechJeb: for the core wrapper, CoreInstance IS the MasterMechJeb
        protected new object MasterMechJeb => CoreInstance;

        public MechJebAscentWrapper Ascent
        {
            get
            {
                if(_ascentWrapper != null) return _ascentWrapper;
                _ascentWrapper = new MechJebAscentWrapper();
                _ascentWrapper.Initialize(this.MasterMechJeb);
                return _ascentWrapper;
            }
        }

        public VesselStateWrapper VesselState {
            get
            {
                if(_vesselStateWrapper != null) return _vesselStateWrapper;
                _vesselStateWrapper = new VesselStateWrapper();
                _vesselStateWrapper.Initialize(this.MasterMechJeb);
                return _vesselStateWrapper;
            }
        }

        [ComputedModule("MechJebModuleInfoItems")]
        public MechJebInfoItemsWrapper InfoItems {
            get
            {
                if(_infoItemsWrapper != null) return _infoItemsWrapper;
                _infoItemsWrapper = new MechJebInfoItemsWrapper();
                _infoItemsWrapper.Initialize(this.MasterMechJeb);
                return _infoItemsWrapper;
            } }

        [ComputedModule("MechJebModuleLandingAutopilot")]
        public MechJebLandingAutopilotWrapper Landing {
            get
            {
                if(_landingWrapper != null) return _landingWrapper;
                _landingWrapper = new MechJebLandingAutopilotWrapper();
                _landingWrapper.Initialize(this.MasterMechJeb);
                return _landingWrapper;
            }
        }

        public Func<object, bool> Running { get; internal set; }

        protected override void InitializeSuffixes()
        {
            this.AddSuffix(new[] { "VESSEL", "VESSELINFO" }, new NoArgsSuffix<VesselStateWrapper>(() => VesselState));
            this.AddSuffix(new[] { "ASCENT", "ASCENTGUIDANCE" }, new NoArgsSuffix<MechJebAscentWrapper>(() => Ascent));
            this.AddSuffix(new[] { "INFO" }, new NoArgsSuffix<MechJebInfoItemsWrapper>(() => InfoItems));
            this.AddSuffix(new[] { "LANDING", "LANDINGAUTOPILOT" }, new NoArgsSuffix<MechJebLandingAutopilotWrapper>(() => Landing));
            this.AddSuffix(new[] { "RUNNING" }, new NoArgsSuffix<BooleanValue>(() => Running(MasterMechJeb)));
        }

        public override string context() => nameof(MechJebCoreWrapper);
    }
}