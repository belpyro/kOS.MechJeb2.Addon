using System.Reflection;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Transfer operations for MechJebManeuverPlannerWrapper
    /// Contains operations for moving between orbits and bodies:
    /// HOHMANN, INTERPLANETARY, COURSECORRECTION, RESONANTORBIT, MOONRETURN
    /// </summary>
    public partial class MechJebManeuverPlannerWrapper
    {
        // Default to MechJeb's GUI behaviour (rendezvous transfer), but allow kOS users to override.
        private bool _hohmannRendezvous = true;

        /// <summary>
        /// Initialize transfer operation suffixes
        /// Called from main InitializeSuffixes() method
        /// </summary>
        partial void InitializeTransferSuffixes()
        {
            AddSuffix("HOHMANN",
                new TwoArgsSuffix<BooleanValue, StringValue, BooleanValue>(
                    HohmannTransfer,
                    "Hohmann transfer to target (timeRef, capture)"));

            AddSuffix("HOHMANNRENDEZVOUS",
                new SetSuffix<BooleanValue>(
                    () => new BooleanValue(_hohmannRendezvous),
                    value => _hohmannRendezvous = (bool)value,
                    "Enable rendezvous targeting for HOHMANN (default TRUE to match MechJeb GUI)."));

            AddSuffix("INTERPLANETARY",
                new OneArgsSuffix<BooleanValue, BooleanValue>(
                    InterplanetaryTransfer,
                    "Interplanetary transfer to target body. Param: waitForPhaseAngle (true=optimal, false=ASAP)"));

            AddSuffix("COURSECORRECTION",
                new OneArgsSuffix<BooleanValue, ScalarValue>(
                    CourseCorrection,
                    "Fine-tune trajectory to target. Adjusts periapsis (bodies) or closest approach (vessels). Auto-timing. Params: finalPeA (m)"));

            AddSuffix("RESONANTORBIT",
                new ThreeArgsSuffix<BooleanValue, ScalarValue, ScalarValue, StringValue>(
                    ResonantOrbit,
                    "Create resonant orbit. Params: numerator, denominator, timeRef (APOAPSIS, PERIAPSIS, etc)"));

            AddSuffix("MOONRETURN",
                new OneArgsSuffix<BooleanValue, ScalarValue>(
                    MoonReturn,
                    "Return from moon orbit to parent body. Params: targetPeriapsis (m). Auto-timing."));
        }

        private BooleanValue HohmannTransfer(StringValue timeRef, BooleanValue capture)
        {
            return ExecuteOperation("OperationGeneric", timeRef, op =>
            {
                // OperationGeneric is the Hohmann transfer operation
                // Set Capture to control whether we get 1 or 2 nodes
                SetBoolFieldOnOperation(op, "Capture", (bool)capture);
                SetBoolFieldOnOperation(op, "PlanCapture", (bool)capture);
                var rendezvous = _hohmannRendezvous;
                SetBoolFieldOnOperation(op, "Rendezvous", rendezvous);
                SetBoolFieldOnOperation(op, "Coplanar", false);    // Full 3D transfer
            });
        }

        private BooleanValue InterplanetaryTransfer(BooleanValue waitForPhaseAngle)
        {
            // No timeRef - MechJeb calculates optimal timing
            return ExecuteOperation("OperationInterplanetaryTransfer", null, op =>
            {
                SetBoolFieldOnOperation(op, "WaitForPhaseAngle", (bool)waitForPhaseAngle);
            });
        }

        private BooleanValue CourseCorrection(ScalarValue finalPeA)
        {
            return ExecuteOperation("OperationCourseCorrection", null, op =>
            {
                // MechJeb's CourseCorrectFinalPeA.Val is in meters (EditableDoubleMult uses
                // multiplier only for GUI display, internal Val is always meters)
                SetEditableOnOperation(op, "CourseCorrectFinalPeA", (double)finalPeA);
            });
        }

        private BooleanValue ResonantOrbit(ScalarValue numerator, ScalarValue denominator, StringValue timeRef)
        {
            return ExecuteOperation("OperationResonantOrbit", timeRef, op =>
            {
                // MechJeb uses EditableInt fields
                SetEditableIntOnOperation(op, "ResonanceNumerator", (int)numerator);
                SetEditableIntOnOperation(op, "ResonanceDenominator", (int)denominator);
            });
        }

        private BooleanValue MoonReturn(ScalarValue targetPeriapsis)
        {
            // No timeRef - MechJeb calculates optimal return window timing
            return ExecuteOperation("OperationMoonReturn", null, op =>
            {
                // MoonReturnAltitude is target periapsis at parent body (in meters)
                SetEditableOnOperation(op, "MoonReturnAltitude", (double)targetPeriapsis);
            });
        }

        /// <summary>
        /// Helper method to set EditableInt fields (used by RESONANTORBIT)
        /// </summary>
        private void SetEditableIntOnOperation(object operation, string fieldName, int value)
        {
            var field = operation.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                var editable = field.GetValue(operation);
                if (editable != null)
                {
                    var valProperty = editable.GetType().GetProperty("Val");
                    if (valProperty != null)
                    {
                        valProperty.SetValue(editable, value);
                    }
                }
            }
        }
    }
}
