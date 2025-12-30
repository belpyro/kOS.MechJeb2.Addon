using System;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using KSPBuildTools;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    /// <summary>
    /// Wrapper for MechJeb's TargetController (Core.Target).
    /// Provides centralized target management for all MechJeb operations.
    /// Access via ADDONS:MJ:CORE:TARGET
    /// </summary>
    [KOSNomenclature("TargetWrapper")]
    public class MechJebTargetWrapper : BaseWrapper
    {
        // Target controller access
        private Func<object, object> _getTargetController;
        private Action<object, object, double, double> _setPositionTarget;
        private Action<object, object> _setTarget;
        private Action<object> _unsetTarget;

        // Property getters
        private Func<object, bool> _getNormalTargetExists;
        private Func<object, bool> _getPositionTargetExists;

        // Group 1: Basic target info
        private Func<object, string> _getName;
        private Func<object, float> _getDistance;
        private Func<object, Vector3d> _getRelativeVelocity;
        private Func<object, Vector3d> _getRelativePosition;
        private Func<object, bool> _getCanAlign;
        private Func<object, Vector3d> _getDockingAxis;

        // Group 2: Target orbit (via TargetOrbit property)
        private Func<object, Orbit> _getTargetOrbit;

        // OrbitExtensions type for rendezvous calculations
        private Type _orbitExtensionsType;

        // PositionTarget type for distinguishing surface markers from vessel targets
        private Type _positionTargetType;

        public override string context() => nameof(MechJebTargetWrapper);

        protected override void BindObject()
        {
            // Get Core.Target (MechJebModuleTargetController)
            _getTargetController = Member(MasterMechJeb, "Target").GetField<object>();

            var targetController = _getTargetController(MasterMechJeb);
            var targetType = targetController.GetType();

            // Bind SetPositionTarget(CelestialBody, double, double)
            var setPositionMethod = targetType.GetMethod("SetPositionTarget",
                new[] { typeof(CelestialBody), typeof(double), typeof(double) });
            _setPositionTarget = (controller, body, lat, lon) =>
                setPositionMethod?.Invoke(controller, new object[] { body, lat, lon });

            // Bind Set(ITargetable)
            var setMethod = targetType.GetMethod("Set", new[] { typeof(ITargetable) });
            _setTarget = (controller, target) =>
                setMethod?.Invoke(controller, new object[] { target });

            // Bind Unset()
            var unsetMethod = targetType.GetMethod("Unset", Type.EmptyTypes);
            _unsetTarget = (controller) =>
                unsetMethod?.Invoke(controller, null);

            // Bind NormalTargetExists property
            var normalTargetExistsProp = targetType.GetProperty("NormalTargetExists");
            if (normalTargetExistsProp != null)
                _getNormalTargetExists = (controller) => (bool)normalTargetExistsProp.GetValue(controller, null);

            // Bind PositionTargetExists property
            var positionTargetExistsProp = targetType.GetProperty("PositionTargetExists");
            if (positionTargetExistsProp != null)
                _getPositionTargetExists = (controller) => (bool)positionTargetExistsProp.GetValue(controller, null);

            // Group 1: Basic target info bindings
            var nameProp = targetType.GetProperty("Name");
            if (nameProp != null)
                _getName = (controller) => (string)nameProp.GetValue(controller, null) ?? "";

            var distanceProp = targetType.GetProperty("Distance");
            if (distanceProp != null)
                _getDistance = (controller) => (float)distanceProp.GetValue(controller, null);

            var relVelProp = targetType.GetProperty("RelativeVelocity");
            if (relVelProp != null)
                _getRelativeVelocity = (controller) => (Vector3d)relVelProp.GetValue(controller, null);

            var relPosProp = targetType.GetProperty("RelativePosition");
            if (relPosProp != null)
                _getRelativePosition = (controller) => (Vector3d)relPosProp.GetValue(controller, null);

            var canAlignProp = targetType.GetProperty("CanAlign");
            if (canAlignProp != null)
                _getCanAlign = (controller) => (bool)canAlignProp.GetValue(controller, null);

            var dockingAxisProp = targetType.GetProperty("DockingAxis");
            if (dockingAxisProp != null)
                _getDockingAxis = (controller) => (Vector3d)dockingAxisProp.GetValue(controller, null);

            // Group 2: Target orbit binding
            var targetOrbitProp = targetType.GetProperty("TargetOrbit");
            if (targetOrbitProp != null)
                _getTargetOrbit = (controller) => (Orbit)targetOrbitProp.GetValue(controller, null);

            // Find OrbitExtensions type for rendezvous calculations
            var mechJebAssembly = targetType.Assembly;
            _orbitExtensionsType = mechJebAssembly.GetType("MuMech.OrbitExtensions");

            // Cache PositionTarget type for type checking (surface markers vs vessels)
            _positionTargetType = mechJebAssembly.GetType("MuMech.PositionTarget");
        }

        protected override void InitializeSuffixes()
        {
            // Position target operations
            AddSuffix("SETTARGET",
                new TwoArgsSuffix<BooleanValue, ScalarValue, ScalarValue>(
                    SetPositionTarget,
                    "Set position target at coordinates (latitude, longitude) on current body"));

            AddSuffix("SETTARGETKSC",
                new NoArgsSuffix<BooleanValue>(
                    SetTargetKSC,
                    "Set position target to KSC runway"));

            // Vessel/Body target operations (by name)
            AddSuffix("SETTARGETVESSEL",
                new OneArgsSuffix<BooleanValue, StringValue>(
                    SetTargetVessel,
                    "Set target to a vessel by name (for rendezvous operations)"));

            AddSuffix("SETTARGETBODY",
                new OneArgsSuffix<BooleanValue, StringValue>(
                    SetTargetBody,
                    "Set target to a celestial body by name (for transfer operations)"));

            AddSuffix("UNSET",
                new NoArgsSuffix<BooleanValue>(
                    UnsetTarget,
                    "Clear the current target"));

            // Read-only target info
            AddSuffix("TARGETLATITUDE",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetLatitude,
                    "Current target latitude in degrees"));

            AddSuffix("TARGETLONGITUDE",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetLongitude,
                    "Current target longitude in degrees"));

            AddSuffix("TARGETBODY",
                new NoArgsSuffix<StringValue>(
                    GetTargetBodyName,
                    "Current target body name"));

            AddSuffix("NORMALTARGETEXISTS",
                new NoArgsSuffix<BooleanValue>(
                    () => HasNormalTarget(),
                    "True if a normal target (vessel or body) is set"));

            AddSuffix("POSITIONTARGETEXISTS",
                new NoArgsSuffix<BooleanValue>(
                    () => HasPositionTarget(),
                    "True if a position target is set"));

            // Group 1: Basic target info
            AddSuffix("NAME",
                new NoArgsSuffix<StringValue>(
                    GetTargetName,
                    "Target name"));

            AddSuffix("DISTANCE",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetDistance,
                    "Distance to target (m)"));

            AddSuffix("RELVELOCITY",
                new NoArgsSuffix<Vector>(
                    GetRelativeVelocity,
                    "Relative velocity vector (m/s)"));

            AddSuffix("RELPOSITION",
                new NoArgsSuffix<Vector>(
                    GetRelativePosition,
                    "Position vector to target (m)"));

            AddSuffix("CANALIGN",
                new NoArgsSuffix<BooleanValue>(
                    GetCanAlign,
                    "True if target supports docking alignment"));

            AddSuffix("DOCKINGAXIS",
                new NoArgsSuffix<Vector>(
                    GetDockingAxis,
                    "Direction to point for docking"));

            // Group 2: Target orbit info
            AddSuffix("TARGETAPOAPSIS",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetApoapsis,
                    "Target orbit apoapsis (m)"));

            AddSuffix("TARGETPERIAPSIS",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetPeriapsis,
                    "Target orbit periapsis (m)"));

            AddSuffix("TARGETINCLINATION",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetInclination,
                    "Target orbit inclination (degrees)"));

            AddSuffix("TARGETECCENTRICITY",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetEccentricity,
                    "Target orbit eccentricity"));

            AddSuffix("TARGETPERIOD",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetPeriod,
                    "Target orbital period (seconds)"));

            AddSuffix("TARGETSMA",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetSMA,
                    "Target semi-major axis (m)"));

            AddSuffix("TARGETLAN",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTargetLAN,
                    "Target longitude of ascending node (degrees)"));

            // Group 3: Rendezvous calculations
            AddSuffix("CLOSESTAPPROACHTIME",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetClosestApproachTime,
                    "Seconds until closest approach"));

            AddSuffix("CLOSESTAPPROACHDISTANCE",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetClosestApproachDistance,
                    "Distance at closest approach (m)"));

            AddSuffix("PHASEANGLE",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetPhaseAngle,
                    "Phase angle to target (degrees)"));

            AddSuffix("RELATIVEINCLINATION",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetRelativeInclination,
                    "Inclination difference (degrees)"));

            // Group 4: Node times
            AddSuffix("TIMETOAN",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTimeToAN,
                    "Seconds to ascending node with target"));

            AddSuffix("TIMETODN",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTimeToDN,
                    "Seconds to descending node with target"));

            AddSuffix("TIMETOEQAN",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTimeToEqAN,
                    "Seconds to equatorial ascending node"));

            AddSuffix("TIMETOEQDN",
                new NoArgsSuffix<ScalarDoubleValue>(
                    GetTimeToEqDN,
                    "Seconds to equatorial descending node"));

            AddSuffix("ANEXISTS",
                new NoArgsSuffix<BooleanValue>(
                    GetANExists,
                    "Does ascending node with target exist?"));

            AddSuffix("DNEXISTS",
                new NoArgsSuffix<BooleanValue>(
                    GetDNExists,
                    "Does descending node with target exist?"));

            // Group 5: Direction target
            AddSuffix("SETDIRECTIONTARGET",
                new OneArgsSuffix<BooleanValue, Vector>(
                    SetDirectionTarget,
                    "Set a direction marker target"));
        }

        private object GetTargetController()
        {
            return _getTargetController(MasterMechJeb);
        }

        /// <summary>
        /// Set position target to specific coordinates on current body.
        /// </summary>
        private BooleanValue SetPositionTarget(ScalarValue latitude, ScalarValue longitude)
        {
            var targetController = GetTargetController();
            var body = FlightGlobals.ActiveVessel.mainBody;

            _setPositionTarget(targetController, body, (double)latitude, (double)longitude);

            Log.Debug($"[kOS.MechJeb2.Addon] Set position target to {latitude}, {longitude} on {body.name}");
            return true;
        }

        /// <summary>
        /// Set position target to KSC runway.
        /// </summary>
        private BooleanValue SetTargetKSC()
        {
            const double kscLatitude = -0.0972;
            const double kscLongitude = -74.5577;

            var targetController = GetTargetController();
            var body = FlightGlobals.GetHomeBody();

            _setPositionTarget(targetController, body, kscLatitude, kscLongitude);

            Log.Debug($"[kOS.MechJeb2.Addon] Set position target to KSC");
            return true;
        }

        /// <summary>
        /// Set target to a vessel by name for rendezvous operations.
        /// </summary>
        private BooleanValue SetTargetVessel(StringValue vesselName)
        {
            if (string.IsNullOrEmpty(vesselName))
            {
                Log.Warning("[kOS.MechJeb2.Addon] SetTargetVessel: Empty vessel name");
                return false;
            }

            var vessel = FlightGlobals.Vessels.FirstOrDefault(v =>
                v.vesselName.Equals(vesselName, StringComparison.OrdinalIgnoreCase));

            if (vessel == null)
            {
                Log.Warning($"[kOS.MechJeb2.Addon] SetTargetVessel: Vessel '{vesselName}' not found");
                return false;
            }

            var targetController = GetTargetController();
            _setTarget(targetController, vessel);

            Log.Debug($"[kOS.MechJeb2.Addon] Set target to vessel: {vessel.vesselName}");
            return true;
        }

        /// <summary>
        /// Set target to a celestial body by name for transfer operations.
        /// </summary>
        private BooleanValue SetTargetBody(StringValue bodyName)
        {
            if (string.IsNullOrEmpty(bodyName))
            {
                Log.Warning("[kOS.MechJeb2.Addon] SetTargetBody: Empty body name");
                return false;
            }

            var body = FlightGlobals.Bodies.FirstOrDefault(b =>
                b.name.Equals(bodyName, StringComparison.OrdinalIgnoreCase));

            if (body == null)
            {
                Log.Warning($"[kOS.MechJeb2.Addon] SetTargetBody: Body '{bodyName}' not found");
                return false;
            }

            var targetController = GetTargetController();
            _setTarget(targetController, body);

            Log.Debug($"[kOS.MechJeb2.Addon] Set target to body: {body.name}");
            return true;
        }

        /// <summary>
        /// Clear the current target.
        /// </summary>
        private BooleanValue UnsetTarget()
        {
            var targetController = GetTargetController();
            _unsetTarget(targetController);

            Log.Debug("[kOS.MechJeb2.Addon] Target unset");
            return true;
        }

        /// <summary>
        /// Get current target latitude.
        /// </summary>
        private ScalarDoubleValue GetTargetLatitude()
        {
            if (!HasPositionTarget()) return 0;

            var targetController = GetTargetController();
            var latField = targetController.GetType().GetField("targetLatitude");
            if (latField != null)
            {
                var editableAngle = latField.GetValue(targetController);
                return GetEditableAngleValue(editableAngle);
            }
            return 0;
        }

        /// <summary>
        /// Get current target longitude.
        /// </summary>
        private ScalarDoubleValue GetTargetLongitude()
        {
            if (!HasPositionTarget()) return 0;

            var targetController = GetTargetController();
            var lonField = targetController.GetType().GetField("targetLongitude");
            if (lonField != null)
            {
                var editableAngle = lonField.GetValue(targetController);
                return GetEditableAngleValue(editableAngle);
            }
            return 0;
        }

        /// <summary>
        /// Get current target body name.
        /// Inspects the actual Target type to determine the correct body:
        /// - CelestialBody targets: returns the target itself
        /// - PositionTarget (surface markers): uses cached targetBody field
        /// - Landed vessels: uses vessel.mainBody
        /// - Orbital vessels: uses TargetOrbit.referenceBody
        /// </summary>
        private StringValue GetTargetBodyName()
        {
            if (!HasAnyTarget()) return "";

            var targetController = GetTargetController();
            var targetProp = targetController.GetType().GetProperty("Target");
            var target = targetProp?.GetValue(targetController, null);

            // 1. CelestialBody target - return the target itself (e.g., targeting Mun returns "Mun")
            if (target is CelestialBody bodyTarget)
                return bodyTarget.name;

            // 2. PositionTarget (surface marker) - use cached targetBody field
            // Note: Must check actual type, not PositionTargetExists (which matches vessels too)
            if (IsPositionTargetType(target))
            {
                var bodyField = targetController.GetType().GetField("targetBody");
                return (bodyField?.GetValue(targetController) as CelestialBody)?.name ?? "";
            }

            // 3. Vessel target - check if landed or orbital
            if (target is Vessel vessel)
            {
                if (vessel.LandedOrSplashed)
                    return vessel.mainBody?.name ?? "";
                // Orbital vessel - use orbit's reference body (e.g., station orbiting Mun returns "Mun")
                var orbitBody = _getTargetOrbit?.Invoke(targetController)?.referenceBody;
                return orbitBody?.name ?? vessel.mainBody?.name ?? "";
            }

            // 4. Fallback to targetBody field
            var fallbackField = targetController.GetType().GetField("targetBody");
            return (fallbackField?.GetValue(targetController) as CelestialBody)?.name ?? "";
        }

        #region Group 1: Basic Target Info

        /// <summary>
        /// Get target name.
        /// </summary>
        private StringValue GetTargetName()
        {
            if (!HasAnyTarget()) return "";
            return _getName?.Invoke(GetTargetController()) ?? "";
        }

        /// <summary>
        /// Get distance to target.
        /// </summary>
        private ScalarDoubleValue GetDistance()
        {
            if (!HasAnyTarget()) return 0;
            return _getDistance?.Invoke(GetTargetController()) ?? 0;
        }

        /// <summary>
        /// Get relative velocity to target as kOS Vector.
        /// </summary>
        private Vector GetRelativeVelocity()
        {
            if (!HasNormalTarget() || _getRelativeVelocity == null)
                return new Vector(0, 0, 0);

            var vel = _getRelativeVelocity(GetTargetController());
            return new Vector(vel.x, vel.y, vel.z);
        }

        /// <summary>
        /// Get relative position to target as kOS Vector.
        /// </summary>
        private Vector GetRelativePosition()
        {
            if (!HasAnyTarget() || _getRelativePosition == null)
                return new Vector(0, 0, 0);

            var pos = _getRelativePosition(GetTargetController());
            return new Vector(pos.x, pos.y, pos.z);
        }

        /// <summary>
        /// Check if target supports docking alignment.
        /// </summary>
        private BooleanValue GetCanAlign()
        {
            if (!HasNormalTarget()) return false;
            return _getCanAlign?.Invoke(GetTargetController()) ?? false;
        }

        /// <summary>
        /// Get docking axis direction as kOS Vector.
        /// </summary>
        private Vector GetDockingAxis()
        {
            if (!HasNormalTarget() || _getDockingAxis == null)
                return new Vector(0, 0, 0);

            try
            {
                var axis = _getDockingAxis(GetTargetController());
                return new Vector(axis.x, axis.y, axis.z);
            }
            catch
            {
                // Target doesn't have a valid docking port
                return new Vector(0, 0, 0);
            }
        }

        #endregion

        #region Group 2: Target Orbit Info

        private Orbit GetTargetOrbitSafe()
        {
            if (!HasNormalTarget() || _getTargetOrbit == null) return null;
            return _getTargetOrbit(GetTargetController());
        }

        private ScalarDoubleValue GetTargetApoapsis()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.ApA ?? 0;
        }

        private ScalarDoubleValue GetTargetPeriapsis()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.PeA ?? 0;
        }

        private ScalarDoubleValue GetTargetInclination()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.inclination ?? 0;
        }

        private ScalarDoubleValue GetTargetEccentricity()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.eccentricity ?? 0;
        }

        private ScalarDoubleValue GetTargetPeriod()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.period ?? 0;
        }

        private ScalarDoubleValue GetTargetSMA()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.semiMajorAxis ?? 0;
        }

        private ScalarDoubleValue GetTargetLAN()
        {
            var orbit = GetTargetOrbitSafe();
            return orbit?.LAN ?? 0;
        }

        #endregion

        #region Group 3: Rendezvous Calculations

        /// <summary>
        /// Get time to closest approach using OrbitExtensions.NextClosestApproachTime
        /// </summary>
        private ScalarDoubleValue GetClosestApproachTime()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("NextClosestApproachTime",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            var result = method.Invoke(null, new object[] { vesselOrbit, targetOrbit, ut });
            return (double)result - ut; // Return time until, not absolute UT
        }

        /// <summary>
        /// Get distance at closest approach using OrbitExtensions.NextClosestApproachDistance
        /// </summary>
        private ScalarDoubleValue GetClosestApproachDistance()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("NextClosestApproachDistance",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            return (double)method.Invoke(null, new object[] { vesselOrbit, targetOrbit, ut });
        }

        /// <summary>
        /// Get phase angle to target using OrbitExtensions.PhaseAngle
        /// </summary>
        private ScalarDoubleValue GetPhaseAngle()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("PhaseAngle",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            return (double)method.Invoke(null, new object[] { vesselOrbit, targetOrbit, ut });
        }

        /// <summary>
        /// Get relative inclination using OrbitExtensions.RelativeInclination
        /// </summary>
        private ScalarDoubleValue GetRelativeInclination()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("RelativeInclination",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit) },
                null);

            if (method == null) return 0;

            return (double)method.Invoke(null, new object[] { vesselOrbit, targetOrbit });
        }

        #endregion

        #region Group 4: Node Times

        /// <summary>
        /// Get time to ascending node with target
        /// </summary>
        private ScalarDoubleValue GetTimeToAN()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("TimeOfAscendingNode",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            var result = method.Invoke(null, new object[] { vesselOrbit, targetOrbit, ut });
            return (double)result - ut;
        }

        /// <summary>
        /// Get time to descending node with target
        /// </summary>
        private ScalarDoubleValue GetTimeToDN()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("TimeOfDescendingNode",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            var result = method.Invoke(null, new object[] { vesselOrbit, targetOrbit, ut });
            return (double)result - ut;
        }

        /// <summary>
        /// Get time to equatorial ascending node
        /// </summary>
        private ScalarDoubleValue GetTimeToEqAN()
        {
            if (_orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("TimeOfAscendingNodeEquatorial",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            var result = method.Invoke(null, new object[] { vesselOrbit, ut });
            return (double)result - ut;
        }

        /// <summary>
        /// Get time to equatorial descending node
        /// </summary>
        private ScalarDoubleValue GetTimeToEqDN()
        {
            if (_orbitExtensionsType == null) return 0;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return 0;

            var method = _orbitExtensionsType.GetMethod("TimeOfDescendingNodeEquatorial",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(double) },
                null);

            if (method == null) return 0;

            var ut = Planetarium.GetUniversalTime();
            var result = method.Invoke(null, new object[] { vesselOrbit, ut });
            return (double)result - ut;
        }

        /// <summary>
        /// Check if ascending node with target exists
        /// </summary>
        private BooleanValue GetANExists()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return false;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return false;

            var method = _orbitExtensionsType.GetMethod("AscendingNodeExists",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit) },
                null);

            if (method == null) return false;

            return (bool)method.Invoke(null, new object[] { vesselOrbit, targetOrbit });
        }

        /// <summary>
        /// Check if descending node with target exists
        /// </summary>
        private BooleanValue GetDNExists()
        {
            var targetOrbit = GetTargetOrbitSafe();
            if (targetOrbit == null || _orbitExtensionsType == null) return false;

            var vesselOrbit = FlightGlobals.ActiveVessel?.orbit;
            if (vesselOrbit == null) return false;

            var method = _orbitExtensionsType.GetMethod("DescendingNodeExists",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Orbit), typeof(Orbit) },
                null);

            if (method == null) return false;

            return (bool)method.Invoke(null, new object[] { vesselOrbit, targetOrbit });
        }

        #endregion

        #region Group 5: Direction Target

        /// <summary>
        /// Set a direction marker target.
        /// First creates a DirectionTarget via SetDirectionTarget(name), then
        /// sets the direction vector via UpdateDirectionTarget(Vector3d).
        /// </summary>
        private BooleanValue SetDirectionTarget(Vector direction)
        {
            var targetController = GetTargetController();
            var targetType = targetController.GetType();

            // Step 1: Create the DirectionTarget by calling SetDirectionTarget(string name)
            var setMethod = targetType.GetMethod("SetDirectionTarget",
                new[] { typeof(string) });

            if (setMethod == null)
            {
                Log.Warning("[kOS.MechJeb2.Addon] SetDirectionTarget method not found");
                return false;
            }

            // Create a DirectionTarget with a descriptive name
            setMethod.Invoke(targetController, new object[] { "kOS Direction" });
            Log.Debug("[kOS.MechJeb2.Addon] DirectionTarget created via SetDirectionTarget");

            // Step 2: Set the direction vector via UpdateDirectionTarget(Vector3d)
            var updateMethod = targetType.GetMethod("UpdateDirectionTarget",
                new[] { typeof(Vector3d) });

            if (updateMethod != null)
            {
                var vec = new Vector3d(direction.X, direction.Y, direction.Z);
                updateMethod.Invoke(targetController, new object[] { vec });
                Log.Debug($"[kOS.MechJeb2.Addon] Direction target vector set: ({direction.X}, {direction.Y}, {direction.Z})");
                return true;
            }

            Log.Warning("[kOS.MechJeb2.Addon] UpdateDirectionTarget method not found");
            return false;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if a normal target exists (vessel, body, or docking port)
        /// </summary>
        private bool HasNormalTarget()
        {
            return _getNormalTargetExists?.Invoke(GetTargetController()) ?? false;
        }

        /// <summary>
        /// Check if a position target exists (surface position or landed vessel)
        /// </summary>
        private bool HasPositionTarget()
        {
            return _getPositionTargetExists?.Invoke(GetTargetController()) ?? false;
        }

        /// <summary>
        /// Check if any target exists (normal or position)
        /// </summary>
        private bool HasAnyTarget()
        {
            return HasNormalTarget() || HasPositionTarget();
        }

        /// <summary>
        /// Check if the target object is a MuMech.PositionTarget (surface marker).
        /// Note: This is different from PositionTargetExists which also matches Vessel targets.
        /// DirectionTarget inherits from PositionTarget, so we check for exact type match.
        /// Uses cached _positionTargetType from assembly for robustness.
        /// </summary>
        private bool IsPositionTargetType(object target)
        {
            if (target == null || _positionTargetType == null) return false;
            return target.GetType() == _positionTargetType;  // Exact type match, not subclasses
        }

        #endregion

        /// <summary>
        /// Extract double value from MechJeb's EditableAngle via reflection.
        /// </summary>
        private double GetEditableAngleValue(object editableAngle)
        {
            if (editableAngle == null) return 0;

            var type = editableAngle.GetType();

            var negativeField = type.GetField("Negative");
            bool negative = negativeField != null && (bool)negativeField.GetValue(editableAngle);

            double degrees = GetEditableDoubleValue(type.GetField("Degrees")?.GetValue(editableAngle));
            double minutes = GetEditableDoubleValue(type.GetField("Minutes")?.GetValue(editableAngle));
            double seconds = GetEditableDoubleValue(type.GetField("Seconds")?.GetValue(editableAngle));

            double value = degrees + minutes / 60.0 + seconds / 3600.0;
            return negative ? -value : value;
        }

        /// <summary>
        /// Extract double value from MechJeb's EditableDouble via reflection.
        /// </summary>
        private double GetEditableDoubleValue(object editableDouble)
        {
            if (editableDouble == null) return 0;

            var valProp = editableDouble.GetType().GetProperty("Val");
            if (valProp != null)
                return (double)valProp.GetValue(editableDouble, null);

            return 0;
        }
    }
}
