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
    public class MechJebCoreWrapper : BaseWrapper
    {
        public Func<object, bool> Running { get; internal set; }

        protected override void InitializeSuffixes()
        {
            this.AddSuffix(new[] { "RUNNING" }, new NoArgsSuffix<BooleanValue>(() => Running(MasterMechJeb)));
        }

        public override string context() => nameof(MechJebCoreWrapper);

        protected override void BindObject()
        {
            Running = Reflect.On(MasterMechJeb).Field("running").AsGetter<bool>();
        }
    }
}