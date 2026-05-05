using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("FuelStatsPair")]
    public class FuelStatsPairValue : Structure
    {
        private readonly MechJebStageStatsWrapper _wrapper;
        private readonly int _stage;

        public FuelStatsPairValue(MechJebStageStatsWrapper wrapper, int stage)
        {
            _wrapper = wrapper;
            _stage = stage;

            AddSuffix(new[] { "VACUUM", "VAC" },
                new NoArgsSuffix<FuelStatsValue>(
                    () => new FuelStatsValue(_wrapper, () => _wrapper.GetVacFuelStats(_stage)),
                    "Vacuum FuelStats for this stage"));

            AddSuffix(new[] { "ATMOSPHERE", "ATMO" },
                new NoArgsSuffix<FuelStatsValue>(
                    () => new FuelStatsValue(_wrapper, () => _wrapper.GetAtmoFuelStats(_stage)),
                    "Atmospheric FuelStats for this stage"));

            AddSuffix("STAGE",
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(_stage),
                    "Fuel-flow stage index this object refers to"));
        }
    }
}
