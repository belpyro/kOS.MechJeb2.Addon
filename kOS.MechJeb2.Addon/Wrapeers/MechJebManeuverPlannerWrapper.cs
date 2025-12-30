using System;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;

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
    public partial class MechJebManeuverPlannerWrapper : BaseWrapper
    {
        // Cache for operation instances (from MechJeb's static array)
        private object[] _operations;
        private Type _operationType;
        private Type _timeSelectorType;
        private Type _timeReferenceType;

        // Method to get ManeuverPlanner module fresh from MasterMechJeb
        private MethodInfo _getComputerModuleMethod;

        // Accessors for ManeuverPlanner module's properties (inherited from ComputerModule)
        private Func<object, object> _getModuleVesselState;  // ComputerModule.VesselState
        private Func<object, object> _getModuleOrbit;        // ComputerModule.Orbit
        private Func<object, object> _getModuleVessel;       // ComputerModule.Vessel
        private Func<object, object> _getCoreTarget;         // ComputerModule.Core.Target
        private Func<object, double> _getVesselStateTime;    // VesselState.time

        /// <summary>
        /// Gets a fresh ManeuverPlanner module from MasterMechJeb every time.
        /// This avoids stale reference issues after save reloads.
        /// </summary>
        private object GetManeuverPlannerModule()
        {
            if (MasterMechJeb == null)
                return null;
            return _getComputerModuleMethod?.Invoke(MasterMechJeb, new object[] { "MechJebModuleManeuverPlanner" });
        }

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

            // Cache the method to get ManeuverPlanner module fresh each time
            // This avoids stale reference issues after save reloads
            // MechJebCore has GetComputerModule(string type) - not GetComputerModule(Type)
            _getComputerModuleMethod = MasterMechJeb.GetType().GetMethod("GetComputerModule",
                new Type[] { typeof(string) });

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

        // Partial method declarations for suffix initialization in partial classes
        partial void InitializeBasicSuffixes();
        partial void InitializeOrbitalSuffixes();
        partial void InitializeTransferSuffixes();
        partial void InitializeRendezvousSuffixes();

        protected override void InitializeSuffixes()
        {
            AddSuffix("OPERATIONS",
                new NoArgsSuffix<ListValue>(
                    GetOperationNames,
                    "List all available maneuver operations"));

            // Initialize suffixes from partial classes
            InitializeBasicSuffixes();
            InitializeOrbitalSuffixes();
            InitializeTransferSuffixes();
            InitializeRendezvousSuffixes();
        }

        public override string context() => nameof(MechJebManeuverPlannerWrapper);

        // Helper method for setting boolean fields (used by partial classes)
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

            // Get a fresh ManeuverPlanner module from MasterMechJeb
            var maneuverPlannerModule = GetManeuverPlannerModule();
            if (maneuverPlannerModule == null)
                throw new KOSException("ManeuverPlanner module not available - MechJeb may not be ready");

            var vessel = _getModuleVessel(maneuverPlannerModule);
            if (vessel == null)
                throw new KOSException("MechJeb vessel reference is invalid");

            // Find the operation
            var operation = _operations.FirstOrDefault(op =>
                op.GetType().Name.Equals(operationTypeName, StringComparison.OrdinalIgnoreCase));

            if (operation == null)
                throw new KOSException($"Operation {operationTypeName} not found");

            // Set time reference on the operation's TimeSelector (skip if null for auto-timing operations)
            if (timeRef != null)
            {
                SetTimeReference(operation, timeRef);
            }

            // Configure operation-specific parameters
            configure(operation);

            // Get orbit and time from the ManeuverPlanner module's properties
            var vesselState = _getModuleVesselState(maneuverPlannerModule);
            var ut = _getVesselStateTime(vesselState);
            var orbit = _getModuleOrbit(maneuverPlannerModule);
            var target = _getCoreTarget(maneuverPlannerModule);

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

            // Check if list is empty (MechJeb returns empty list for invalid transfer windows)
            if (nodes.Count == 0)
                return false;

            foreach (var node in nodes)
            {
                var dV = GetField(node, "dV");
                var nodeUT = (double)GetField(node, "UT");
                _placeManeuverNodeMethod.Invoke(null, new[] { vessel, orbit, dV, nodeUT });
            }

            return true;
        }

        /// <summary>
        /// Execute an operation that requires setting targetLongitude on the target controller.
        /// Used by LONGITUDE and LAN operations.
        /// </summary>
        private BooleanValue ExecuteOperationWithTargetLongitude(string operationTypeName, double longitudeDegrees, StringValue timeRef)
        {
            if (!Initialized)
                throw new KOSException("ManeuverPlanner not initialized");

            // Get a fresh ManeuverPlanner module from MasterMechJeb
            var maneuverPlannerModule = GetManeuverPlannerModule();
            if (maneuverPlannerModule == null)
                throw new KOSException("ManeuverPlanner module not available - MechJeb may not be ready");

            var vessel = _getModuleVessel(maneuverPlannerModule);
            if (vessel == null)
                throw new KOSException("MechJeb vessel reference is invalid");

            // Find the operation
            var operation = _operations.FirstOrDefault(op =>
                op.GetType().Name.Equals(operationTypeName, StringComparison.OrdinalIgnoreCase));

            if (operation == null)
                throw new KOSException($"Operation {operationTypeName} not found");

            // Set time reference on the operation's TimeSelector
            if (timeRef != null)
            {
                SetTimeReference(operation, timeRef);
            }

            // Get orbit, time, and target controller
            var vesselState = _getModuleVesselState(maneuverPlannerModule);
            var ut = _getVesselStateTime(vesselState);
            var orbit = _getModuleOrbit(maneuverPlannerModule);
            var target = _getCoreTarget(maneuverPlannerModule);

            if (target == null)
                throw new KOSException("Target controller not available");

            // Set targetLongitude on the target controller
            // targetLongitude is an EditableAngle field - we can create a new instance from a double
            var targetLongitudeField = target.GetType().GetField("targetLongitude",
                BindingFlags.Public | BindingFlags.Instance);

            if (targetLongitudeField == null)
                throw new KOSException("targetLongitude field not found on target controller");

            var editableAngle = targetLongitudeField.GetValue(target);
            if (editableAngle == null)
                throw new KOSException("targetLongitude is null");

            // Create a new EditableAngle from the longitude value
            // EditableAngle has a constructor that takes a double and does all the D/M/S decomposition
            var editableAngleType = editableAngle.GetType();
            var newAngle = Activator.CreateInstance(editableAngleType, longitudeDegrees);
            targetLongitudeField.SetValue(target, newAngle);

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
            var nodes = (System.Collections.IList)nodeList;

            // Check if list is empty
            if (nodes.Count == 0)
                return false;

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
