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
        public MechJebAscentWrapper Ascent { get; } = new MechJebAscentWrapper();

        public VesselStateWrapper VesselState { get; } = new VesselStateWrapper();

        [ComputedModule("MechJebModuleInfoItems")]
        public MechJebInfoItemsWrapper InfoItems { get; } = new MechJebInfoItemsWrapper();

        public Func<object, bool> Running { get; internal set; }

        protected override void BindObject()
        {
            this.BindCoreWrapper(CoreInstance);
        }

        protected override void InitializeSuffixes()
        {
            this.AddSuffix(new[] { "VESSEL", "VESSELINFO" }, new NoArgsSuffix<VesselStateWrapper>(() => VesselState));
            this.AddSuffix(new[] { "ASCENT", "ASCENTGUIDANCE" }, new NoArgsSuffix<MechJebAscentWrapper>(() => Ascent));
            this.AddSuffix(new[] { "INFO" }, new NoArgsSuffix<MechJebInfoItemsWrapper>(() => InfoItems));
            this.AddSuffix(new[] { "RUNNING" }, new NoArgsSuffix<BooleanValue>(() => Running(CoreInstance)));
        }

        public override string context() => nameof(MechJebCoreWrapper);
    }
}