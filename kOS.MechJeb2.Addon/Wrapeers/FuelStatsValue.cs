using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("FuelStats")]
    public class FuelStatsValue : Structure
    {
        public FuelStatsValue(MechJebStageStatsWrapper wrapper, Func<object> snapshotProvider)
        {
            foreach (var accessor in wrapper.Accessors)
            {
                var captured = accessor;
                AddSuffix(captured.Name,
                    new NoArgsSuffix<ScalarDoubleValue>(
                        () => new ScalarDoubleValue(captured.Getter(snapshotProvider()))));
            }
        }
    }
}
