using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Rendezvous operations for MechJebManeuverPlannerWrapper
    /// Contains operations for matching/aligning with targets:
    /// PLANE, KILLRELVEL, CHANGEINCLINATION, LAMBERT
    /// </summary>
    public partial class MechJebManeuverPlannerWrapper
    {
        /// <summary>
        /// Initialize rendezvous operation suffixes
        /// Called from main InitializeSuffixes() method
        /// </summary>
        partial void InitializeRendezvousSuffixes()
        {
            AddSuffix("PLANE",
                new OneArgsSuffix<BooleanValue, StringValue>(
                    MatchPlane,
                    "Match orbital plane with target. Requires target. TimeRef: REL_NEAREST_AD, REL_HIGHEST_AD, REL_ASCENDING, REL_DESCENDING"));

            AddSuffix("KILLRELVEL",
                new OneArgsSuffix<BooleanValue, StringValue>(
                    KillRelativeVelocity,
                    "Match velocity with target. Automatically calculates deltaV. Params: timeRef (CLOSEST_APPROACH, X_FROM_NOW)"));

            AddSuffix("CHANGEINCLINATION",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    ChangeInclination,
                    "Change orbital inclination. Params: newInclination (degrees), timeRef (EQ_ASCENDING, EQ_DESCENDING, etc)"));

            AddSuffix("LAMBERT",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    Lambert,
                    "Lambert intercept trajectory. Requires target in same SOI. Params: interceptInterval (seconds), timeRef (X_FROM_NOW)"));

            // *TIMED variants - for X_FROM_NOW with explicit seconds (thread-safe)
            AddSuffix("KILLRELVELTIMED",
                new TwoArgsSuffix<BooleanValue, StringValue, ScalarValue>(
                    KillRelativeVelocityWithTime,
                    "Match velocity with target using X_FROM_NOW seconds"));

            AddSuffix("CHANGEINCLINATIONTIMED",
                new ThreeArgsSuffix<BooleanValue, ScalarValue, StringValue, ScalarValue>(
                    ChangeInclinationWithTime,
                    "Change inclination with X_FROM_NOW seconds"));

            AddSuffix("LAMBERTTIMED",
                new ThreeArgsSuffix<BooleanValue, ScalarValue, StringValue, ScalarValue>(
                    LambertWithTime,
                    "Lambert intercept with X_FROM_NOW seconds"));
        }

        private BooleanValue MatchPlane(StringValue timeRef)
        {
            // No parameters - all behavior from timeRef (REL_ASCENDING, etc)
            return ExecuteOperation("OperationPlane", timeRef, op => { });
        }

        private BooleanValue KillRelativeVelocity(StringValue timeRef)
        {
            return ExecuteOperation("OperationKillRelVel", timeRef, op => { });
        }

        private BooleanValue ChangeInclination(ScalarValue newInc, StringValue timeRef)
        {
            return ExecuteOperation("OperationInclination", timeRef, op =>
            {
                SetEditableOnOperation(op, "NewInc", (double)newInc);
            });
        }

        private BooleanValue Lambert(ScalarValue interceptInterval, StringValue timeRef)
        {
            return ExecuteOperation("OperationLambert", timeRef, op =>
            {
                // InterceptInterval is time between burn and intercept (seconds)
                SetEditableOnOperation(op, "InterceptInterval", (double)interceptInterval);
            });
        }

        // ============================================================================
        // *TIMED variants - thread-safe X_FROM_NOW support
        // ============================================================================

        private BooleanValue KillRelativeVelocityWithTime(StringValue timeRef, ScalarValue xFromNowSeconds)
        {
            return ExecuteOperation("OperationKillRelVel", timeRef, op => { }, (double)xFromNowSeconds);
        }

        private BooleanValue ChangeInclinationWithTime(ScalarValue newInc, StringValue timeRef, ScalarValue xFromNowSeconds)
        {
            return ExecuteOperation("OperationInclination", timeRef, op =>
            {
                SetEditableOnOperation(op, "NewInc", (double)newInc);
            }, (double)xFromNowSeconds);
        }

        private BooleanValue LambertWithTime(ScalarValue interceptInterval, StringValue timeRef, ScalarValue xFromNowSeconds)
        {
            return ExecuteOperation("OperationLambert", timeRef, op =>
            {
                SetEditableOnOperation(op, "InterceptInterval", (double)interceptInterval);
            }, (double)xFromNowSeconds);
        }
    }
}
