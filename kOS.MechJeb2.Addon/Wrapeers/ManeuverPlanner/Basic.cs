using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Basic orbital operations for MechJebManeuverPlannerWrapper
    /// Contains fundamental single-orbit adjustments: CHANGEPE, CHANGEAP, CIRCULARIZE, ELLIPTICIZE, SEMIMAJOR
    /// </summary>
    public partial class MechJebManeuverPlannerWrapper
    {
        /// <summary>
        /// Initialize basic operation suffixes
        /// Called from main InitializeSuffixes() method
        /// </summary>
        partial void InitializeBasicSuffixes()
        {
            AddSuffix("CHANGEPE",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    ChangePeriapsis,
                    "Change periapsis to altitude (m) at time reference (APOAPSIS, PERIAPSIS, etc)"));

            AddSuffix("CHANGEAP",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    ChangeApoapsis,
                    "Change apoapsis to altitude (m) at time reference"));

            AddSuffix("CIRCULARIZE",
                new OneArgsSuffix<BooleanValue, StringValue>(
                    Circularize,
                    "Circularize at time reference (APOAPSIS, PERIAPSIS, etc)"));

            AddSuffix("ELLIPTICIZE",
                new ThreeArgsSuffix<BooleanValue, ScalarValue, ScalarValue, StringValue>(
                    Ellipticize,
                    "Set both periapsis and apoapsis. Params: newPeA (m), newApA (m), timeRef"));

            AddSuffix("SEMIMAJOR",
                new TwoArgsSuffix<BooleanValue, ScalarValue, StringValue>(
                    SemiMajor,
                    "Change semi-major axis to size (m) at time reference"));
        }

        private BooleanValue ChangePeriapsis(ScalarValue altitude, StringValue timeRef)
        {
            return ExecuteOperation("OperationPeriapsis", timeRef, op =>
            {
                // Use same pattern as AscentWrapper's BindEditable
                SetEditableOnOperation(op, "NewPeA", (double)altitude);
            });
        }

        private BooleanValue ChangeApoapsis(ScalarValue altitude, StringValue timeRef)
        {
            return ExecuteOperation("OperationApoapsis", timeRef, op =>
            {
                // Use same pattern as AscentWrapper's BindEditable
                SetEditableOnOperation(op, "NewApA", (double)altitude);
            });
        }

        private BooleanValue Circularize(StringValue timeRef)
        {
            return ExecuteOperation("OperationCircularize", timeRef, op => { });
        }

        private BooleanValue Ellipticize(ScalarValue newPeA, ScalarValue newApA, StringValue timeRef)
        {
            return ExecuteOperation("OperationEllipticize", timeRef, op =>
            {
                // MechJeb uses kilometers internally, but we pass meters and MechJeb converts
                SetEditableOnOperation(op, "NewPeA", (double)newPeA);
                SetEditableOnOperation(op, "NewApA", (double)newApA);
            });
        }

        private BooleanValue SemiMajor(ScalarValue newSma, StringValue timeRef)
        {
            return ExecuteOperation("OperationSemiMajor", timeRef, op =>
            {
                // MechJeb uses kilometers internally, but we pass meters and MechJeb converts
                SetEditableOnOperation(op, "NewSma", (double)newSma);
            });
        }
    }
}
