using System;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("AscentWrapper")]
    public class MechJebAscentWrapper : BaseWrapper, IMechJebAscentWrapper
    {
        private Func<object, object> _ascentSettingsGetter;
        private Func<object, object> _stagingControllerGetter;
        private Func<object, object> _thrustControllerGetter;
        private Func<object, object> _nodeExecutorGetter;
        private Func<object, object> _autopilotGetter;

        // Users pool for proper autopilot engagement (like MechJeb GUI does)
        private Func<object, object> _usersGetter;
        private Action<object, object> _usersAdd;
        private Action<object, object> _usersRemove;
        private readonly object _userIdentity = new object(); // Sentinel object to identify this wrapper as a user

        protected override void BindObject()
        {
            _ascentSettingsGetter = Member(MasterMechJeb, "AscentSettings").GetField<object>();
            _stagingControllerGetter = Member(MasterMechJeb, "Staging").GetField<object>();
            _thrustControllerGetter = Member(MasterMechJeb, "Thrust").GetField<object>();
            _nodeExecutorGetter = Member(MasterMechJeb, "Node").GetField<object>();
            _autopilotGetter = Member(MasterMechJeb, "Ascent").GetProp<object>();

            var ascentSettings = _ascentSettingsGetter(MasterMechJeb);
            var stagingController = _stagingControllerGetter(MasterMechJeb);
            var thrustController = _thrustControllerGetter(MasterMechJeb);
            var nodeExecutor = _nodeExecutorGetter(MasterMechJeb);
            var autopilot = _autopilotGetter(MasterMechJeb);

            GetEnabled = Member(autopilot, nameof(Enabled)).GetProp<bool>();
            SetEnabled = Member(autopilot, nameof(Enabled)).SetProp<bool>();

            // Bind to Users pool for proper autopilot engagement
            // MechJeb GUI uses _autopilot.Users.Add(this) to engage, not Enabled = true directly
            // Note: Users is a field, not a property (public readonly UserPool Users)
            _usersGetter = Member(autopilot, "Users").GetField<object>();
            var users = _usersGetter(autopilot);
            _usersAdd = Reflect.OnType(users.GetType()).Method("Add").WithArgs(typeof(object)).AsAction();
            _usersRemove = Reflect.OnType(users.GetType()).Method("Remove").WithArgs(typeof(object)).AsAction();

            (GetDesiredAltitudeDouble, SetDesiredAltitude) =
                BindEditable<double>(ascentSettings, "DesiredOrbitAltitude");

            GetAutoPath = Member(ascentSettings, "AutoPath").GetField<bool>();
            SetAutoPath = Member(ascentSettings, "AutoPath").SetField<bool>();

            GetAscentTypeInteger = Member(ascentSettings, "AscentTypeInteger").GetField<int>();

            (GetTurnStartAltitudeDouble, SetTurnStartAltitude) =
                BindEditable<double>(ascentSettings, "TurnStartAltitude");

            GetAutostage = Member(ascentSettings, "Autostage").GetProp<bool>();
            SetAutostage = Member(ascentSettings, "Autostage").SetProp<bool>();

            (GetTurnStartVelocityDouble, SetTurnStartVelocity) =
                BindEditable<double>(ascentSettings, "TurnStartVelocity");
            (GetTurnEndAltitudeDouble, SetTurnEndAltitude) = BindEditable<double>(ascentSettings, "TurnEndAltitude");
            (GetTurnEndAngleDouble, SetTurnEndAngle) = BindEditable<double>(ascentSettings, "TurnEndAngle");
            (GetTurnShapeExponentDouble, SetTurnShapeExponent) =
                BindEditable<double>(ascentSettings, "TurnShapeExponent");

            GetAutoDeployAntennas = Member(ascentSettings, "AutoDeployAntennas").GetField<bool>();
            SetAutoDeployAntennas = Member(ascentSettings, "AutoDeployAntennas").SetField<bool>();

            GetAutodeploySolarPanels = Member(ascentSettings, "AutodeploySolarPanels").GetField<bool>();
            SetAutodeploySolarPanels = Member(ascentSettings, "AutodeploySolarPanels").SetField<bool>();

            GetSkipCircularization = Member(ascentSettings, "SkipCircularization").GetField<bool>();
            SetSkipCircularization = Member(ascentSettings, "SkipCircularization").SetField<bool>();

            (GetAutostageLimit, SetAutostageLimit) = BindEditable<int>(stagingController, "AutostageLimit");
            (GetAutostagePreDelay, SetAutostagePreDelay) =
                BindEditable<double>(stagingController, "AutostagePreDelay");
            (GetAutostagePostDelay, SetAutostagePostDelay) =
                BindEditable<double>(stagingController, "AutostagePostDelay");
            (GetFairingMaxAerothermalFlux, SetFairingMaxAerothermalFlux) =
                BindEditable<double>(stagingController, "FairingMaxAerothermalFlux");
            (GetFairingMaxDynamicPressure, SetFairingMaxDynamicPressure) =
                BindEditable<double>(stagingController, "FairingMaxDynamicPressure");
            (GetFairingMinAltitude, SetFairingMinAltitude) =
                BindEditable<double>(stagingController, "FairingMinAltitude");

            GetHotStaging = Member(stagingController, "HotStaging").GetField<bool>();
            SetHotStaging = Member(stagingController, "HotStaging").SetField<bool>();

            (GetHotStagingLeadTime, SetHotStagingLeadTime) =
                BindEditable<double>(stagingController, "HotStagingLeadTime");

            GetDropSolids = Member(stagingController, "DropSolids").GetField<bool>();
            SetDropSolids = Member(stagingController, "DropSolids").SetField<bool>();

            (GetDropSolidsLeadTime, SetDropSolidsLeadTime) =
                BindEditable<double>(stagingController, "DropSolidsLeadTime");
            (GetClampAutoStageThrustPct, SetClampAutoStageThrustPct) =
                BindEditable<double>(stagingController, "ClampAutoStageThrustPct");

            // --- NEW: Desired inclination (EditableDoubleMult DesiredInclination)
            (GetDesiredInclinationDouble, SetDesiredInclination) =
                BindEditable<double>(ascentSettings, "DesiredInclination");
            // --- NEW: Corrective steering
            GetCorrectiveSteering = Member(ascentSettings, "CorrectiveSteering").GetField<bool>();
            SetCorrectiveSteering = Member(ascentSettings, "CorrectiveSteering").SetField<bool>();

            (GetCorrectiveSteeringGainDouble, SetCorrectiveSteeringGain) =
                BindEditable<double>(ascentSettings, "CorrectiveSteeringGain");

            // --- NEW: Roll settings
            (GetVerticalRollDouble, SetVerticalRoll) =
                BindEditable<double>(ascentSettings, "VerticalRoll");

            (GetTurnRollDouble, SetTurnRoll) =
                BindEditable<double>(ascentSettings, "TurnRoll");

            (GetRollAltitudeDouble, SetRollAltitude) =
                BindEditable<double>(ascentSettings, "RollAltitude");

            GetForceRoll = Member(ascentSettings, "ForceRoll").GetField<bool>();
            SetForceRoll = Member(ascentSettings, "ForceRoll").SetField<bool>();
            // --- NEW: AoA & Q limits
            GetLimitAoA = Member(ascentSettings, "LimitAoA").GetField<bool>();
            SetLimitAoA = Member(ascentSettings, "LimitAoA").SetField<bool>();

            (GetMaxAoADouble, SetMaxAoA) =
                BindEditable<double>(ascentSettings, "MaxAoA");

            (GetAoALimitFadeoutPressureDouble, SetAoALimitFadeoutPressure) =
                BindEditable<double>(ascentSettings, "AOALimitFadeoutPressure");

            (GetLimitQaDouble, SetLimitQa) =
                BindEditable<double>(thrustController, "MaxDynamicPressure");

            GetLimitQaEnabled = Member(thrustController, "LimitDynamicPressure").GetField<bool>();
            SetLimitQaEnabled = Member(thrustController, "LimitDynamicPressure").SetField<bool>();

            GetLimitToPreventOverheats = Member(thrustController, "LimitToPreventOverheats").GetField<bool>();
            SetLimitToPreventOverheats = Member(thrustController, "LimitToPreventOverheats").SetField<bool>();

            GetAutoWarp = Member(nodeExecutor, "AutoWarp").GetField<bool>();
            SetAutoWarp = Member(nodeExecutor, "AutoWarp").SetField<bool>();
        }

        protected override void InitializeSuffixes()
        {
            AddSuffix("ENABLED",
                new SetSuffix<BooleanValue>(() => Enabled, value => Enabled = value, "Is Ascent autopilot enable?"));
            AddSuffix(new[] { "DESIREDALTITUDE", "DSRALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetDesiredAltitudeDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetDesiredAltitude(_ascentSettingsGetter(MasterMechJeb), value),
                    "Desired Altitude"));
            AddSuffix(new[] { "TURNSTARTALTITUDE", "STARTALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnStartAltitudeDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnStartAltitude(_ascentSettingsGetter(MasterMechJeb), value),
                    "Turn Start Altitude"));
            AddSuffix(new[] { "TURNSTARTVELOCITY", "STARTV" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnStartVelocityDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnStartVelocity(_ascentSettingsGetter(MasterMechJeb), value),
                    "Turn Start Velocity"));
            AddSuffix(new[] { "TURNENDALTITUDE", "ENDALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnEndAltitudeDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnEndAltitude(_ascentSettingsGetter(MasterMechJeb), value),
                    "Turn End Altitude"));
            AddSuffix(new[] { "TURNENDANGLE", "ENDANG" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnEndAngleDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnEndAngle(_ascentSettingsGetter(MasterMechJeb), value),
                    "Turn End Angle"));
            AddSuffix(new[] { "TURNSHAPEEXPONENT", "TSHAPEEXP" },
                new ClampSetSuffix<ScalarDoubleValue>(
                    () => GetTurnShapeExponentDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnShapeExponent(_ascentSettingsGetter(MasterMechJeb), value), min: 0, max: 1,
                    stepIncrement: 0f,
                    "Turn Shape Exponent"));
            AddSuffix(new[] { "AUTOPATH" },
                new SetSuffix<BooleanValue>(() => GetAutoPath(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetAutoPath(_ascentSettingsGetter(MasterMechJeb), value), "Automatic Altitude Turn"));
            AddSuffix(new[] { "AUTOSTAGE" },
                new SetSuffix<BooleanValue>(() => GetAutostage(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetAutostage(_ascentSettingsGetter(MasterMechJeb), value), "Auto Stage Status"));
            AddSuffix(new[] { "ASCENTTYPE", "ASCTYPE" },
                new NoArgsSuffix<StringValue>(() => AscentType == 0 ? "CLASSIC" : "NOT SUPPORTED"));
            AddSuffix(new[] { "AUTODEPLOYANTENNAS", "AUTODANT" },
                new SetSuffix<BooleanValue>(() => GetAutoDeployAntennas(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetAutoDeployAntennas(_ascentSettingsGetter(MasterMechJeb), value),
                    "Automatically deploy antennas after launch"));
            AddSuffix(new[] { "AUTODEPLOYSOLARPANELS", "AUTODSOL" },
                new SetSuffix<BooleanValue>(() => GetAutodeploySolarPanels(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetAutodeploySolarPanels(_ascentSettingsGetter(MasterMechJeb), value),
                    "Automatically deploy solar panels after launch"));
            AddSuffix(new[] { "SKIPCIRCULARIZATION", "SKIPCIRC" },
                new SetSuffix<BooleanValue>(() => GetSkipCircularization(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetSkipCircularization(_ascentSettingsGetter(MasterMechJeb), value),
                    "Skip circularization burn at apoapsis"));

            // --- Autostage settings (staging controller) ---
            AddSuffix(new[] { "AUTOSTAGELIMIT", "ASTGLIM" },
                new SetSuffix<ScalarIntValue>(
                    () => GetAutostageLimit(_stagingControllerGetter(MasterMechJeb)),
                    value => SetAutostageLimit(_stagingControllerGetter(MasterMechJeb), value),
                    "Maximum number of stages to auto-stage"));

            AddSuffix(new[] { "AUTOSTAGEPREDELAY", "ASTGPRE" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAutostagePreDelay(_stagingControllerGetter(MasterMechJeb)),
                    value => SetAutostagePreDelay(_stagingControllerGetter(MasterMechJeb), value),
                    "Delay before staging (seconds)"));

            AddSuffix(new[] { "AUTOSTAGEPOSTDELAY", "ASTGPOST" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAutostagePostDelay(_stagingControllerGetter(MasterMechJeb)),
                    value => SetAutostagePostDelay(_stagingControllerGetter(MasterMechJeb), value),
                    "Delay after staging (seconds)"));

            // Fairing jettison conditions
            AddSuffix(new[] { "FAIRINGMAXDYNAMICPRESSURE", "FMAXQ" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMaxDynamicPressure(_stagingControllerGetter(MasterMechJeb)),
                    value => SetFairingMaxDynamicPressure(_stagingControllerGetter(MasterMechJeb), value),
                    "Maximum dynamic pressure for fairing jettison"));

            AddSuffix(new[] { "FAIRINGMINALTITUDE", "FMINALT" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMinAltitude(_stagingControllerGetter(MasterMechJeb)),
                    value => SetFairingMinAltitude(_stagingControllerGetter(MasterMechJeb), value),
                    "Minimum altitude for fairing jettison"));

            AddSuffix(new[] { "FAIRINGMAXAEROTHERMALFLUX", "FMAXFLUX" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMaxAerothermalFlux(_stagingControllerGetter(MasterMechJeb)),
                    value => SetFairingMaxAerothermalFlux(_stagingControllerGetter(MasterMechJeb), value),
                    "Maximum aerothermal flux for fairing jettison"));

            // Hot staging
            AddSuffix(new[] { "HOTSTAGING", "HOTSTAGE" },
                new SetSuffix<BooleanValue>(
                    () => GetHotStaging(_stagingControllerGetter(MasterMechJeb)),
                    value => SetHotStaging(_stagingControllerGetter(MasterMechJeb), value),
                    "Enable hot staging"));
            AddSuffix(new[] { "HOTSTAGINGLEADTIME", "HOTLEAD" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetHotStagingLeadTime(_stagingControllerGetter(MasterMechJeb)),
                    value => SetHotStagingLeadTime(_stagingControllerGetter(MasterMechJeb), value),
                    "Hot staging lead time (seconds)"));

            // Drop solids
            AddSuffix(new[] { "DROPSOLIDS", "DRPSLD" },
                new SetSuffix<BooleanValue>(
                    () => GetDropSolids(_stagingControllerGetter(MasterMechJeb)),
                    value => SetDropSolids(_stagingControllerGetter(MasterMechJeb), value),
                    "Enable dropping of solid boosters"));
            AddSuffix(new[] { "DROPSOLIDSLEADTIME", "SLDSLEAD" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetDropSolidsLeadTime(_stagingControllerGetter(MasterMechJeb)),
                    value => SetDropSolidsLeadTime(_stagingControllerGetter(MasterMechJeb), value),
                    "Lead time before dropping solids (seconds)"));

            // Clamp AutoStage thrust percent
            AddSuffix(new[] { "CLAMPAUTOSTAGETHRUSTPCT", "CLAMPTHRUST" },
                new ClampSetSuffix<ScalarDoubleValue>(
                    () => GetClampAutoStageThrustPct(_stagingControllerGetter(MasterMechJeb)),
                    value => SetClampAutoStageThrustPct(_stagingControllerGetter(MasterMechJeb), value),
                    min: 0, max: 1, stepIncrement: 0f,
                    "Minimum thrust percent required to auto-stage"));
            // --- Classic orbit target: inclination ---
            AddSuffix(new[] { "DESIREDINCLINATION", "INC" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetDesiredInclinationDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetDesiredInclination(_ascentSettingsGetter(MasterMechJeb), value),
                    "Desired orbital inclination (degrees)"));

            // --- Corrective steering ---
            AddSuffix(new[] { "CORRECTIVESTEERING", "CSTEER" },
                new SetSuffix<BooleanValue>(
                    () => GetCorrectiveSteering(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetCorrectiveSteering(_ascentSettingsGetter(MasterMechJeb), value),
                    "Enable classic corrective steering"));

            AddSuffix(new[] { "CORRECTIVESTEERINGGAIN", "CSTEERGAIN" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetCorrectiveSteeringGainDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetCorrectiveSteeringGain(_ascentSettingsGetter(MasterMechJeb), value),
                    "Corrective steering gain"));

            // --- Roll profile ---
            AddSuffix(new[] { "FORCEROLL", "FROLL" },
                new SetSuffix<BooleanValue>(
                    () => GetForceRoll(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetForceRoll(_ascentSettingsGetter(MasterMechJeb), value),
                    "Force roll during ascent"));

            AddSuffix(new[] { "VERTICALROLL", "VROLL" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetVerticalRollDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetVerticalRoll(_ascentSettingsGetter(MasterMechJeb), value),
                    "Roll angle during vertical ascent (degrees)"));

            AddSuffix(new[] { "TURNROLL", "TROLL" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetTurnRollDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetTurnRoll(_ascentSettingsGetter(MasterMechJeb), value),
                    "Roll angle during gravity turn (degrees)"));

            AddSuffix(new[] { "ROLLALTITUDE", "ROLLALT" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetRollAltitudeDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetRollAltitude(_ascentSettingsGetter(MasterMechJeb), value),
                    "Altitude at which roll is applied"));

            // --- AoA & Q limits ---
            AddSuffix(new[] { "LIMITAOA", "LIMAOA" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitAoA(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetLimitAoA(_ascentSettingsGetter(MasterMechJeb), value),
                    "Enable angle-of-attack limit"));

            AddSuffix(new[] { "MAXAOA", "MAXAOA" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetMaxAoADouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetMaxAoA(_ascentSettingsGetter(MasterMechJeb), value),
                    "Maximum angle of attack (degrees)"));

            AddSuffix(new[] { "AOALIMITFADEOUTPRESSURE", "AOAFADEQ" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAoALimitFadeoutPressureDouble(_ascentSettingsGetter(MasterMechJeb)),
                    value => SetAoALimitFadeoutPressure(_ascentSettingsGetter(MasterMechJeb), value),
                    "Pressure at which AoA limit fades out"));

            AddSuffix(new[] { "LIMITQA", "LIMQA" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetLimitQaDouble(_thrustControllerGetter(MasterMechJeb)),
                    value => SetLimitQa(_thrustControllerGetter(MasterMechJeb), value),
                    "Dynamic pressure limit (Q)"));

            AddSuffix(new[] { "LIMITQAENABLED", "LIMQAEN" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitQaEnabled(_thrustControllerGetter(MasterMechJeb)),
                    value => SetLimitQaEnabled(_thrustControllerGetter(MasterMechJeb), value),
                    "Enable dynamic pressure limit (Q)"));

            // --- Thrust controller safety limits ---
            AddSuffix(new[] { "LIMITTOPREVENTOVERHEATS", "LIMOVHT" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitToPreventOverheats(_thrustControllerGetter(MasterMechJeb)),
                    value => SetLimitToPreventOverheats(_thrustControllerGetter(MasterMechJeb), value),
                    "Limit thrust to prevent engine overheating"));

            // --- Node executor auto-warp ---
            AddSuffix(new[] { "AUTOWARP", "AWARP" },
                new SetSuffix<BooleanValue>(
                    () => GetAutoWarp(_nodeExecutorGetter(MasterMechJeb)),
                    value => SetAutoWarp(_nodeExecutorGetter(MasterMechJeb), value),
                    "Enable automatic time warp for maneuver execution"));
        }

        public override string context() => nameof(MechJebAscentWrapper);

        public BooleanValue Enabled
        {
            get =>
                Initialized
                    ? new BooleanValue(GetEnabled(_autopilotGetter(MasterMechJeb)))
                    : throw new KOSException("Cannot get Enabled property of not initialized MechJebAscentWrapper");
            set
            {
                if (!Initialized) return;

                var autopilot = _autopilotGetter(MasterMechJeb);
                var users = _usersGetter(autopilot);

                // Use Users.Add/Remove like MechJeb GUI does, instead of setting Enabled directly
                // This properly engages/disengages the autopilot
                if (value)
                    _usersAdd(users, _userIdentity);
                else
                    _usersRemove(users, _userIdentity);
            }
        }


        public int AscentType => GetAscentTypeInteger(_ascentSettingsGetter(MasterMechJeb));

        private Func<object, bool> GetEnabled { get; set; }
        private Action<object, bool> SetEnabled { get; set; }

        private Func<object, double> GetDesiredAltitudeDouble { get; set; }

        private Action<object, double> SetDesiredAltitude { get; set; }

        private Func<object, bool> GetAutoPath { get; set; }
        private Action<object, bool> SetAutoPath { get; set; }

        private Func<object, int> GetAscentTypeInteger { get; set; }

        public Action<object, bool> SetAutoWarp { get; set; }

        public Func<object, bool> GetAutoWarp { get; set; }

        //Autostage
        private Func<object, bool> GetAutostage { get; set; }
        private Action<object, bool> SetAutostage { get; set; }

        //TurnStartAltitude EditableDoubleMult
        private Func<object, double> GetTurnStartAltitudeDouble { get; set; }

        private Action<object, double> SetTurnStartAltitude { get; set; }

        //TurnStartVelocity EditableDoubleMult
        private Func<object, double> GetTurnStartVelocityDouble { get; set; }

        private Action<object, double> SetTurnStartVelocity { get; set; }

        //TurnEndAltitude 
        private Func<object, double> GetTurnEndAltitudeDouble { get; set; }

        private Action<object, double> SetTurnEndAltitude { get; set; }

        //TurnEndAngle
        private Func<object, double> GetTurnEndAngleDouble { get; set; }

        private Action<object, double> SetTurnEndAngle { get; set; }

        //TurnShapeExponent
        private Func<object, double> GetTurnShapeExponentDouble { get; set; }
        private Action<object, double> SetTurnShapeExponent { get; set; }

        //AutodeploySolarPanels
        private Func<object, bool> GetAutodeploySolarPanels { get; set; }

        private Action<object, bool> SetAutodeploySolarPanels { get; set; }

        //AutoDeployAntennas
        private Func<object, bool> GetAutoDeployAntennas { get; set; }
        private Action<object, bool> SetAutoDeployAntennas { get; set; }

        //SkipCircularization
        private Func<object, bool> GetSkipCircularization { get; set; }
        private Action<object, bool> SetSkipCircularization { get; set; }

        // Autostage limit (EditableInt AutostageLimit)
        public Func<object, int> GetAutostageLimit { get; set; }
        public Action<object, int> SetAutostageLimit { get; set; }

        // Delays (EditableDouble AutostagePreDelay / AutostagePostDelay)
        public Func<object, double> GetAutostagePreDelay { get; set; }
        public Action<object, double> SetAutostagePreDelay { get; set; }

        public Func<object, double> GetAutostagePostDelay { get; set; }
        public Action<object, double> SetAutostagePostDelay { get; set; }

        // Fairing conditions (EditableDoubleMult.*.Val)
        public Func<object, double> GetFairingMaxDynamicPressure { get; set; }
        public Action<object, double> SetFairingMaxDynamicPressure { get; set; }

        public Func<object, double> GetFairingMinAltitude { get; set; }
        public Action<object, double> SetFairingMinAltitude { get; set; }

        public Func<object, double> GetFairingMaxAerothermalFlux { get; set; }
        public Action<object, double> SetFairingMaxAerothermalFlux { get; set; }

        // Hot staging (bool HotStaging + EditableDouble HotStagingLeadTime)
        public Func<object, bool> GetHotStaging { get; set; }
        public Action<object, bool> SetHotStaging { get; set; }

        public Func<object, double> GetHotStagingLeadTime { get; set; }
        public Action<object, double> SetHotStagingLeadTime { get; set; }

        // Drop solids (bool DropSolids + EditableDouble DropSolidsLeadTime)
        public Func<object, bool> GetDropSolids { get; set; }
        public Action<object, bool> SetDropSolids { get; set; }

        public Func<object, double> GetDropSolidsLeadTime { get; set; }
        public Action<object, double> SetDropSolidsLeadTime { get; set; }

        // Clamp AutoStage Thrust (EditableDouble ClampAutoStageThrustPct)
        public Func<object, double> GetClampAutoStageThrustPct { get; set; }
        public Action<object, double> SetClampAutoStageThrustPct { get; set; }

        // Desired inclination
        private Func<object, double> GetDesiredInclinationDouble { get; set; }
        private Action<object, double> SetDesiredInclination { get; set; }

        // Corrective steering
        private Func<object, bool> GetCorrectiveSteering { get; set; }
        private Action<object, bool> SetCorrectiveSteering { get; set; }

        private Func<object, double> GetCorrectiveSteeringGainDouble { get; set; }
        private Action<object, double> SetCorrectiveSteeringGain { get; set; }

        // Roll settings
        private Func<object, bool> GetForceRoll { get; set; }
        private Action<object, bool> SetForceRoll { get; set; }

        private Func<object, double> GetVerticalRollDouble { get; set; }
        private Action<object, double> SetVerticalRoll { get; set; }

        private Func<object, double> GetTurnRollDouble { get; set; }
        private Action<object, double> SetTurnRoll { get; set; }

        private Func<object, double> GetRollAltitudeDouble { get; set; }
        private Action<object, double> SetRollAltitude { get; set; }

        // AoA & Q limits
        private Func<object, bool> GetLimitAoA { get; set; }
        private Action<object, bool> SetLimitAoA { get; set; }

        private Func<object, double> GetMaxAoADouble { get; set; }
        private Action<object, double> SetMaxAoA { get; set; }

        private Func<object, double> GetAoALimitFadeoutPressureDouble { get; set; }
        private Action<object, double> SetAoALimitFadeoutPressure { get; set; }

        private Func<object, double> GetLimitQaDouble { get; set; }
        private Action<object, double> SetLimitQa { get; set; }

        private Func<object, bool> GetLimitQaEnabled { get; set; }
        private Action<object, bool> SetLimitQaEnabled { get; set; }

        //LimitToPreventOverheats
        private Func<object, bool> GetLimitToPreventOverheats { get; set; }
        private Action<object, bool> SetLimitToPreventOverheats { get; set; }

        //Helpers
        private (Func<object, T> get, Action<object, T> set)
            BindEditable<T>(object settings, string fieldName)
        {
            var fieldCtx = Reflect.On(settings).Field(fieldName);

            var getterObj = fieldCtx.AsGetter<object>();
            var getterDouble = Reflect.On(getterObj(settings))
                .Property(Constants.EditableDoubleValueName)
                .AsGetter<T>();

            var setVal = Reflect.On(getterObj(settings))
                .Property(Constants.EditableDoubleValueName)
                .AsSetter<T>();

            return (ctx => getterDouble(getterObj(ctx)), (ctx, val) => setVal(getterObj(ctx), val));
        }
    }
}