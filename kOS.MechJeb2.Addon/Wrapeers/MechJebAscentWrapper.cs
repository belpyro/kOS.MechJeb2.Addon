using System;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("AscentWrapper")]
    public class MechJebAscentWrapper : BaseWrapper, IMechJebAscentWrapper
    {
        public override void Initialize(object coreInstance)
        {
            if(Initialized) return;
            base.Initialize(coreInstance);
            this.BindCoreWrapper(coreInstance);
            RegisterInitializer(InitializeSuffixes);
        }

        private void InitializeSuffixes()
        {
            AddSuffix("ENABLED", new NoArgsSuffix<BooleanValue>(() => Enabled(CoreInstance), "Is Ascent autopilot enable?"));
        }
        public Func<object, bool> Enabled { get; set; }
    }
}