using System;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using UnityEngine;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Wrapper for MechJeb's Maneuver Planner operations.
    /// Allows creating maneuver nodes for various orbital operations.
    ///
    /// Available operations in MechJeb:
    /// - OperationApoapsis - change apoapsis
    /// - OperationPeriapsis - change periapsis
    /// - OperationCircularize - circularize orbit
    /// - OperationEllipticize - set both Ap and Pe
    /// - OperationInclination - change inclination
    /// - OperationLan - change longitude of ascending node
    /// - OperationPlane - match planes with target
    /// - OperationTransfer - Hohmann transfer to target
    /// - OperationMoonReturn - return from moon
    /// - OperationInterplanetaryTransfer - transfer to another planet
    /// - OperationCourseCorrection - fine tune trajectory
    /// - OperationKillRelVel - match velocity with target
    /// - OperationLambert - intercept target
    /// - OperationResonantOrbit - create resonant orbit
    /// - OperationSemiMajor - change semi-major axis
    /// - OperationLongitude - change longitude of periapsis
    /// - OperationEccentricity - change eccentricity
    ///
    /// TimeReference options:
    /// - APOAPSIS, PERIAPSIS - at next Ap/Pe
    /// - X_FROM_NOW - after specified time
    /// - ALTITUDE - at specified altitude
    /// - EQ_ASCENDING, EQ_DESCENDING - at equatorial nodes
    /// - REL_ASCENDING, REL_DESCENDING - at relative nodes (with target)
    /// - CLOSEST_APPROACH - at closest approach to target
    /// </summary>
    [KOSNomenclature("ManeuverPlannerWrapper")]
    public class MechJebManeuverPlannerWrapper : BaseWrapper, IMechJebManeuverPlannerWrapper
    {
        // Cache for operation instances (from MechJeb's static array)
        private object[] _operations;
        private Type _operationType;
        private Type _timeSelectorType;
        private Type _timeReferenceType;

        // The actual ManeuverPlanner module - we use ITS properties for context
        private object _maneuverPlannerModule;

        // Accessors for ManeuverPlanner module's properties (inherited from ComputerModule)
        private Func<object, object> _getModuleVesselState;  // ComputerModule.VesselState
        private Func<object, object> _getModuleOrbit;        // ComputerModule.Orbit
        private Func<object, object> _getModuleVessel;       // ComputerModule.Vessel
        private Func<object, object> _getCoreTarget;         // ComputerModule.Core.Target
        private Func<object, double> _getVesselStateTime;    // VesselState.time

        // For placing nodes
        private MethodInfo _placeManeuverNodeMethod;

        protected override void BindObject()
        {
            // Access MechJeb's actual static operations array - same instances the GUI uses
            // This ensures our changes are visible in MechJeb UI and we use proven instances
            _operationType = "MuMech.Operation".GetTypeFromCache();
            var plannerType = "MuMech.MechJebModuleManeuverPlanner".GetTypeFromCache();
            var opsField = plannerType.GetField("_operation",
                BindingFlags.NonPublic | BindingFlags.Static);
            _operations = (object[])opsField.GetValue(null);

            // Get the actual ManeuverPlanner module from MasterMechJeb (MechJebCore)
            // We use this module's properties exactly like WindowGUI does
            // MechJebCore has GetComputerModule(string type) - not GetComputerModule(Type)
            var getComputerModuleMethod = MasterMechJeb.GetType().GetMethod("GetComputerModule",
                new Type[] { typeof(string) });
            _maneuverPlannerModule = getComputerModuleMethod.Invoke(MasterMechJeb, new object[] { "MechJebModuleManeuverPlanner" });

            // Find TimeSelector and TimeReference types
            _timeSelectorType = "MuMech.TimeSelector".GetTypeFromCache();
            _timeReferenceType = "MuMech.TimeReference".GetTypeFromCache();

            // Bind accessors for ManeuverPlanner module's ComputerModule properties
            // These are the EXACT same properties that WindowGUI uses (VesselState, Orbit, Vessel)
            var computerModuleType = "MuMech.ComputerModule".GetTypeFromCache();

            // VesselState property (from ComputerModule: => Core.VesselState)
            _getModuleVesselState = Reflect.OnType(computerModuleType).Property("VesselState").AsGetter<object>();

            // Orbit property (from ComputerModule: => Part.vessel.orbit)
            _getModuleOrbit = Reflect.OnType(computerModuleType).Property("Orbit").AsGetter<object>();

            // Vessel property (from DisplayModule/ComputerModule)
            _getModuleVessel = Reflect.OnType(computerModuleType).Property("Vessel").AsGetter<object>();

            // Core.Target (ComputerModule.Core is a field, MechJebCore.Target is a field)
            var coreGetter = Reflect.OnType(computerModuleType).Field("Core").AsGetter<object>();
            var targetGetter = Reflect.OnType(MasterMechJeb.GetType()).Field("Target").AsGetter<object>();
            _getCoreTarget = (module) =>
            {
                var core = coreGetter(module);
                return targetGetter(core);
            };

            // VesselState.time field
            _getVesselStateTime = Reflect.OnType("MuMech.VesselState".GetTypeFromCache()).Field("time").AsGetter<double>();

            // Get PlaceManeuverNode method
            var vesselExtType = "MuMech.VesselExtensions".GetTypeFromCache();
            _placeManeuverNodeMethod = vesselExtType.GetMethod("PlaceManeuverNode",
                BindingFlags.Public | BindingFlags.Static);
        }

        protected override void InitializeSuffixes()
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

            AddSuffix("HOHMANN",
                new TwoArgsSuffix<BooleanValue, StringValue, BooleanValue>(
                    HohmannTransfer,
                    "Hohmann transfer to target (timeRef, capture)"));

            AddSuffix("OPERATIONS",
                new NoArgsSuffix<ListValue>(
                    GetOperationNames,
                    "List all available maneuver operations"));
        }

        public override string context() => nameof(MechJebManeuverPlannerWrapper);

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

        private BooleanValue HohmannTransfer(StringValue timeRef, BooleanValue capture)
        {
            return ExecuteOperation("OperationGeneric", timeRef, op =>
            {
                // OperationGeneric is the Hohmann transfer operation
                // Set Capture to control whether we get 1 or 2 nodes
                SetBoolFieldOnOperation(op, "Capture", (bool)capture);
                SetBoolFieldOnOperation(op, "PlanCapture", (bool)capture);
                SetBoolFieldOnOperation(op, "Rendezvous", false);  // Transfer, not rendezvous
                SetBoolFieldOnOperation(op, "Coplanar", false);    // Full 3D transfer
            });
        }

        private void SetBoolFieldOnOperation(object operation, string fieldName, bool value)
        {
            var field = operation.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(operation, value);
            }
        }

        private ListValue GetOperationNames()
        {
            var list = new ListValue();
            foreach (var op in _operations)
            {
                var getName = op.GetType().GetMethod("GetName");
                var name = (string)getName.Invoke(op, null);
                list.Add(new StringValue(name));
            }
            return list;
        }

        private BooleanValue ExecuteOperation(string operationTypeName, StringValue timeRef, Action<object> configure)
        {
            if (!Initialized)
                throw new KOSException("ManeuverPlanner not initialized");

            // Find the operation
            var operation = _operations.FirstOrDefault(op =>
                op.GetType().Name.Equals(operationTypeName, StringComparison.OrdinalIgnoreCase));

            if (operation == null)
                throw new KOSException($"Operation {operationTypeName} not found");

            // Set time reference on the operation's TimeSelector
            SetTimeReference(operation, timeRef);

            // Configure operation-specific parameters
            configure(operation);

            // Get orbit and time from the ManeuverPlanner module's properties
            // This is EXACTLY what WindowGUI does: VesselState.time and Orbit
            if (_maneuverPlannerModule == null)
                throw new KOSException("ManeuverPlanner module not available");

            var vesselState = _getModuleVesselState(_maneuverPlannerModule);
            var ut = _getVesselStateTime(vesselState);
            var orbit = _getModuleOrbit(_maneuverPlannerModule);
            var vessel = _getModuleVessel(_maneuverPlannerModule);
            var target = _getCoreTarget(_maneuverPlannerModule);

            // Call MakeNodes - exactly like WindowGUI line 104
            var makeNodes = operation.GetType().GetMethod("MakeNodes");
            var nodeList = makeNodes.Invoke(operation, new object[] { orbit, ut, target });

            if (nodeList == null)
            {
                // Check ErrorMessage via GetErrorMessage() method
                var getErrorMsg = operation.GetType().GetMethod("GetErrorMessage");
                var errorMsg = getErrorMsg?.Invoke(operation, null) as string;
                if (!string.IsNullOrEmpty(errorMsg))
                    throw new KOSException($"Maneuver failed: {errorMsg}");
                return false;
            }

            // Place the nodes using Vessel.PlaceManeuverNode - exactly like WindowGUI line 111
            // ManeuverParameters has dV (Vector3d) and UT (double) as fields
            var nodes = (System.Collections.IList)nodeList;

            foreach (var node in nodes)
            {
                var dV = GetField(node, "dV");
                var nodeUT = (double)GetField(node, "UT");
                _placeManeuverNodeMethod.Invoke(null, new[] { vessel, orbit, dV, nodeUT });
            }

            return true;
        }

        private void SetTimeReference(object operation, StringValue timeRefName)
        {
            // Get the _timeSelector field (it's static in Operation subclasses)
            var timeSelectorField = operation.GetType()
                .GetField("_timeSelector", BindingFlags.NonPublic | BindingFlags.Static);

            if (timeSelectorField == null)
            {
                // Try instance field as fallback
                timeSelectorField = operation.GetType()
                    .GetField("_timeSelector", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (timeSelectorField == null)
                throw new KOSException("Operation does not have a TimeSelector");

            var timeSelector = timeSelectorField.GetValue(timeSelectorField.IsStatic ? null : operation);

            // Parse the time reference enum
            var timeRefValue = Enum.Parse(_timeReferenceType, timeRefName.ToString().ToUpper());

            // Find the index of this time reference in _allowedTimeRef
            var allowedTimeRefField = _timeSelectorType
                .GetField("_allowedTimeRef", BindingFlags.NonPublic | BindingFlags.Instance);
            var allowedTimeRef = (Array)allowedTimeRefField.GetValue(timeSelector);

            int index = -1;
            for (int i = 0; i < allowedTimeRef.Length; i++)
            {
                if (allowedTimeRef.GetValue(i).Equals(timeRefValue))
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
                throw new KOSException($"TimeReference {timeRefName} not allowed for this operation");

            // Set _currentTimeRef (it's public due to [Persistent] attribute)
            var currentTimeRefField = _timeSelectorType
                .GetField("_currentTimeRef", BindingFlags.Public | BindingFlags.Instance);
            if (currentTimeRefField == null)
            {
                currentTimeRefField = _timeSelectorType
                    .GetField("_currentTimeRef", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            currentTimeRefField.SetValue(timeSelector, index);
        }

        /// <summary>
        /// Sets a value on an EditableDoubleMult field using the Val property.
        /// Now that we use MechJeb's actual instances, this should work correctly.
        /// </summary>
        private void SetEditableOnOperation(object operation, string fieldName, double value)
        {
            // Get the EditableDoubleMult field from the operation
            var editableField = operation.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var editable = editableField?.GetValue(operation);

            if (editable == null)
                throw new KOSException($"Field {fieldName} not found or null");

            // Use same pattern as AscentWrapper - set via Val property
            var setter = Reflect.On(editable).Property("Val").AsSetter<double>();
            setter(editable, value);
        }

        private object GetField(object obj, string name)
        {
            var field = obj.GetType().GetField(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(obj);
        }

        private object GetProperty(object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return prop?.GetValue(obj);
        }
    }
}
