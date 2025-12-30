using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Orbital geometry operations for MechJebManeuverPlannerWrapper
    /// Contains operations that modify orbital shape parameters:
    /// ECCENTRICITY, LONGITUDE, LAN
    /// </summary>
    public partial class MechJebManeuverPlannerWrapper
    {
        /// <summary>
        /// Initialize orbital geometry operation suffixes
        /// Called from main InitializeSuffixes() method
        /// </summary>
        partial void InitializeOrbitalSuffixes()
        {
            AddSuffix("ECCENTRICITY",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    Eccentricity,
                    "Change orbital eccentricity (0=circular, <1=elliptical) at time reference"));

            AddSuffix("LONGITUDE",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    Longitude,
                    "Change longitude of periapsis. Params: targetLongitude (degrees), timeRef (APOAPSIS, PERIAPSIS)"));

            AddSuffix("LAN",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    Lan,
                    "Change longitude of ascending node. Params: targetLan (degrees), timeRef (APOAPSIS, PERIAPSIS, X_FROM_NOW)"));
        }

        private BooleanValue Eccentricity(ScalarValue newEcc, StringValue timeRef)
        {
            return ExecuteOperation("OperationEccentricity", timeRef, op =>
            {
                // Eccentricity is unitless (0-1 range)
                SetEditableOnOperation(op, "NewEcc", (double)newEcc);
            });
        }

        private BooleanValue Longitude(ScalarValue targetLongDegrees, StringValue timeRef)
        {
            // OperationLongitude reads longitude from target.targetLongitude
            // We need to set it before calling MakeNodes
            return ExecuteOperationWithTargetLongitude("OperationLongitude", (double)targetLongDegrees, timeRef);
        }

        private BooleanValue Lan(ScalarValue targetLanDegrees, StringValue timeRef)
        {
            // OperationLan reads longitude from target.targetLongitude (yes, LAN uses targetLongitude field)
            // We need to set it before calling MakeNodes
            return ExecuteOperationWithTargetLongitude("OperationLan", (double)targetLanDegrees, timeRef);
        }
    }
}
