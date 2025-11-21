using System;
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
        private object _coreModule;
        private object _ascentSettings;
        private object _stagingController;
        private object _thrustController;
        private object _nodeExecutor;

        public override void Initialize(object coreInstance)
        {
            if (Initialized) return;
            base.Initialize(coreInstance);
            RegisterInitializer(InitializeSuffixes);
        }

        protected override void BindObject()
        {
            _coreModule = Member(CoreInstance, "Core").GetField<object>()(CoreInstance);
            _ascentSettings = Member(_coreModule, "AscentSettings").GetField<object>()(_coreModule);
            _stagingController = Member(_coreModule, "Staging").GetField<object>()(_coreModule);
            _thrustController = Member(_coreModule, "Thrust").GetField<object>()(_coreModule);
            _nodeExecutor = Member(_coreModule, "Node").GetField<object>()(_coreModule);

            GetEnabled = Member(CoreInstance, nameof(Enabled)).GetProp<bool>();
            SetEnabled = Member(CoreInstance, nameof(Enabled)).SetProp<bool>();

            (GetDesiredAltitudeDouble, SetDesiredAltitude) =
                BindEditable<double>(_ascentSettings, "DesiredOrbitAltitude");

            GetAutoPath = Member(_ascentSettings, "AutoPath").GetField<bool>();
            SetAutoPath = Member(_ascentSettings, "AutoPath").SetField<bool>();

            GetAscentTypeInteger = Member(_ascentSettings, "AscentTypeInteger").GetField<int>();

            (GetTurnStartAltitudeDouble, SetTurnStartAltitude) =
                BindEditable<double>(_ascentSettings, "TurnStartAltitude");

            GetAutostage = Member(_ascentSettings, "Autostage").GetProp<bool>();
            SetAutostage = Member(_ascentSettings, "Autostage").SetProp<bool>();

            (GetTurnStartVelocityDouble, SetTurnStartVelocity) =
                BindEditable<double>(_ascentSettings, "TurnStartVelocity");
            (GetTurnEndAltitudeDouble, SetTurnEndAltitude) = BindEditable<double>(_ascentSettings, "TurnEndAltitude");
            (GetTurnEndAngleDouble, SetTurnEndAngle) = BindEditable<double>(_ascentSettings, "TurnEndAngle");
            (GetTurnShapeExponentDouble, SetTurnShapeExponent) =
                BindEditable<double>(_ascentSettings, "TurnShapeExponent");

            GetAutoDeployAntennas = Member(_ascentSettings, "AutoDeployAntennas").GetField<bool>();
            SetAutoDeployAntennas = Member(_ascentSettings, "AutoDeployAntennas").SetField<bool>();

            GetAutodeploySolarPanels = Member(_ascentSettings, "AutodeploySolarPanels").GetField<bool>();
            SetAutodeploySolarPanels = Member(_ascentSettings, "AutodeploySolarPanels").SetField<bool>();

            GetSkipCircularization = Member(_ascentSettings, "SkipCircularization").GetField<bool>();
            SetSkipCircularization = Member(_ascentSettings, "SkipCircularization").SetField<bool>();

            (GetAutostageLimit, SetAutostageLimit) = BindEditable<int>(_stagingController, "AutostageLimit");
            (GetAutostagePreDelay, SetAutostagePreDelay) =
                BindEditable<double>(_stagingController, "AutostagePreDelay");
            (GetAutostagePostDelay, SetAutostagePostDelay) =
                BindEditable<double>(_stagingController, "AutostagePostDelay");
            (GetFairingMaxAerothermalFlux, SetFairingMaxAerothermalFlux) =
                BindEditable<double>(_stagingController, "FairingMaxAerothermalFlux");
            (GetFairingMaxDynamicPressure, SetFairingMaxDynamicPressure) =
                BindEditable<double>(_stagingController, "FairingMaxDynamicPressure");
            (GetFairingMinAltitude, SetFairingMinAltitude) =
                BindEditable<double>(_stagingController, "FairingMinAltitude");

            GetHotStaging = Member(_stagingController, "HotStaging").GetField<bool>();
            SetHotStaging = Member(_stagingController, "HotStaging").SetField<bool>();

            (GetHotStagingLeadTime, SetHotStagingLeadTime) =
                BindEditable<double>(_stagingController, "HotStagingLeadTime");

            GetDropSolids = Member(_stagingController, "DropSolids").GetField<bool>();
            SetDropSolids = Member(_stagingController, "DropSolids").SetField<bool>();

            (GetDropSolidsLeadTime, SetDropSolidsLeadTime) =
                BindEditable<double>(_stagingController, "DropSolidsLeadTime");
            (GetClampAutoStageThrustPct, SetClampAutoStageThrustPct) =
                BindEditable<double>(_stagingController, "ClampAutoStageThrustPct");
            
            // --- NEW: Desired inclination (EditableDoubleMult DesiredInclination)
            (GetDesiredInclinationDouble, SetDesiredInclination) =
                BindEditable<double>(_ascentSettings, "DesiredInclination");
            // --- NEW: Corrective steering
            GetCorrectiveSteering = Member(_ascentSettings, "CorrectiveSteering").GetField<bool>();
            SetCorrectiveSteering = Member(_ascentSettings, "CorrectiveSteering").SetField<bool>();

            (GetCorrectiveSteeringGainDouble, SetCorrectiveSteeringGain) =
                BindEditable<double>(_ascentSettings, "CorrectiveSteeringGain");

            // --- NEW: Roll settings
            (GetVerticalRollDouble, SetVerticalRoll) =
                BindEditable<double>(_ascentSettings, "VerticalRoll");

            (GetTurnRollDouble, SetTurnRoll) =
                BindEditable<double>(_ascentSettings, "TurnRoll");

            (GetRollAltitudeDouble, SetRollAltitude) =
                BindEditable<double>(_ascentSettings, "RollAltitude");

            GetForceRoll = Member(_ascentSettings, "ForceRoll").GetField<bool>();
            SetForceRoll = Member(_ascentSettings, "ForceRoll").SetField<bool>();
            // --- NEW: AoA & Q limits
            GetLimitAoA = Member(_ascentSettings, "LimitAoA").GetField<bool>();
            SetLimitAoA = Member(_ascentSettings, "LimitAoA").SetField<bool>();

            (GetMaxAoADouble, SetMaxAoA) =
                BindEditable<double>(_ascentSettings, "MaxAoA");

            (GetAoALimitFadeoutPressureDouble, SetAoALimitFadeoutPressure) =
                BindEditable<double>(_ascentSettings, "AOALimitFadeoutPressure");

            (GetLimitQaDouble, SetLimitQa) =
                BindEditable<double>(_thrustController,"MaxDynamicPressure");

            GetLimitQaEnabled = Member(_thrustController, "LimitDynamicPressure").GetField<bool>();
            SetLimitQaEnabled = Member(_thrustController, "LimitDynamicPressure").SetField<bool>();
            
            GetLimitToPreventOverheats = Member(_thrustController, "LimitToPreventOverheats").GetField<bool>();
            SetLimitToPreventOverheats = Member(_thrustController, "LimitToPreventOverheats").SetField<bool>();
            
            GetAutoWarp =  Member(_nodeExecutor, "AutoWarp").GetField<bool>();
            SetAutoWarp =  Member(_nodeExecutor, "AutoWarp").SetField<bool>();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("ENABLED",
                new SetSuffix<BooleanValue>(() => Enabled, value => Enabled = value, "Is Ascent autopilot enable?"));
            AddSuffix(new[] { "DESIREDALTITUDE", "DSRALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetDesiredAltitudeDouble(_ascentSettings),
                    value => SetDesiredAltitude(_ascentSettings, value),
                    "Desired Altitude"));
            AddSuffix(new[] { "TURNSTARTALTITUDE", "STARTALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnStartAltitudeDouble(_ascentSettings),
                    value => SetTurnStartAltitude(_ascentSettings, value),
                    "Turn Start Altitude"));
            AddSuffix(new[] { "TURNSTARTVELOCITY", "STARTV" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnStartVelocityDouble(_ascentSettings),
                    value => SetTurnStartVelocity(_ascentSettings, value),
                    "Turn Start Velocity"));
            AddSuffix(new[] { "TURNENDALTITUDE", "ENDALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnEndAltitudeDouble(_ascentSettings),
                    value => SetTurnEndAltitude(_ascentSettings, value),
                    "Turn End Altitude"));
            AddSuffix(new[] { "TURNENDANGLE", "ENDANG" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnEndAngleDouble(_ascentSettings),
                    value => SetTurnEndAngle(_ascentSettings, value),
                    "Turn End Angle"));
            AddSuffix(new[] { "TURNSHAPEEXPONENT", "TSHAPEEXP" },
                new ClampSetSuffix<ScalarDoubleValue>(() => GetTurnShapeExponentDouble(_ascentSettings),
                    value => SetTurnShapeExponent(_ascentSettings, value), min: 0, max: 1, stepIncrement: 0f,
                    "Turn Shape Exponent"));
            AddSuffix(new[] { "AUTOPATH" },
                new SetSuffix<BooleanValue>(() => GetAutoPath(_ascentSettings),
                    value => SetAutoPath(_ascentSettings, value), "Automatic Altitude Turn"));
            AddSuffix(new[] { "AUTOSTAGE" },
                new SetSuffix<BooleanValue>(() => GetAutostage(_ascentSettings),
                    value => SetAutostage(_ascentSettings, value), "Auto Stage Status"));
            AddSuffix(new[] { "ASCENTTYPE", "ASCTYPE" },
                new NoArgsSuffix<StringValue>(() => AscentType == 0 ? "CLASSIC" : "NOT SUPPORTED"));
            AddSuffix(new[] { "AUTODEPLOYANTENNAS", "AUTODANT" },
                new SetSuffix<BooleanValue>(() => GetAutoDeployAntennas(_ascentSettings),
                    value => SetAutoDeployAntennas(_ascentSettings, value),
                    "Automatically deploy antennas after launch"));
            AddSuffix(new[] { "AUTODEPLOYSOLARPANELS", "AUTODSOL" },
                new SetSuffix<BooleanValue>(() => GetAutodeploySolarPanels(_ascentSettings),
                    value => SetAutodeploySolarPanels(_ascentSettings, value),
                    "Automatically deploy solar panels after launch"));
            AddSuffix(new[] { "SKIPCIRCULARIZATION", "SKIPCIRC" },
                new SetSuffix<BooleanValue>(() => GetSkipCircularization(_ascentSettings),
                    value => SetSkipCircularization(_ascentSettings, value),
                    "Skip circularization burn at apoapsis"));

            // --- Autostage settings (staging controller) ---
            AddSuffix(new[] { "AUTOSTAGELIMIT", "ASTGLIM" },
                new SetSuffix<ScalarIntValue>(
                    () => GetAutostageLimit(_stagingController),
                    value => SetAutostageLimit(_stagingController, value),
                    "Maximum number of stages to auto-stage"));

            AddSuffix(new[] { "AUTOSTAGEPREDELAY", "ASTGPRE" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAutostagePreDelay(_stagingController),
                    value => SetAutostagePreDelay(_stagingController, value),
                    "Delay before staging (seconds)"));

            AddSuffix(new[] { "AUTOSTAGEPOSTDELAY", "ASTGPOST" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAutostagePostDelay(_stagingController),
                    value => SetAutostagePostDelay(_stagingController, value),
                    "Delay after staging (seconds)"));

            // Fairing jettison conditions
            AddSuffix(new[] { "FAIRINGMAXDYNAMICPRESSURE", "FMAXQ" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMaxDynamicPressure(_stagingController),
                    value => SetFairingMaxDynamicPressure(_stagingController, value),
                    "Maximum dynamic pressure for fairing jettison"));

            AddSuffix(new[] { "FAIRINGMINALTITUDE", "FMINALT" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMinAltitude(_stagingController),
                    value => SetFairingMinAltitude(_stagingController, value),
                    "Minimum altitude for fairing jettison"));

            AddSuffix(new[] { "FAIRINGMAXAEROTHERMALFLUX", "FMAXFLUX" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetFairingMaxAerothermalFlux(_stagingController),
                    value => SetFairingMaxAerothermalFlux(_stagingController, value),
                    "Maximum aerothermal flux for fairing jettison"));

            // Hot staging
            AddSuffix(new[] { "HOTSTAGING", "HOTSTAGE" },
                new SetSuffix<BooleanValue>(
                    () => GetHotStaging(_stagingController),
                    value => SetHotStaging(_stagingController, value),
                    "Enable hot staging"));
            AddSuffix(new[] { "HOTSTAGINGLEADTIME", "HOTLEAD" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetHotStagingLeadTime(_stagingController),
                    value => SetHotStagingLeadTime(_stagingController, value),
                    "Hot staging lead time (seconds)"));

            // Drop solids
            AddSuffix(new[] { "DROPSOLIDS", "DRPSLD" },
                new SetSuffix<BooleanValue>(
                    () => GetDropSolids(_stagingController),
                    value => SetDropSolids(_stagingController, value),
                    "Enable dropping of solid boosters"));
            AddSuffix(new[] { "DROPSOLIDSLEADTIME", "SLDSLEAD" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetDropSolidsLeadTime(_stagingController),
                    value => SetDropSolidsLeadTime(_stagingController, value),
                    "Lead time before dropping solids (seconds)"));

            // Clamp AutoStage thrust percent
            AddSuffix(new[] { "CLAMPAUTOSTAGETHRUSTPCT", "CLAMPTHRUST" },
                new ClampSetSuffix<ScalarDoubleValue>(
                    () => GetClampAutoStageThrustPct(_stagingController),
                    value => SetClampAutoStageThrustPct(_stagingController, value),
                    min: 0, max: 1, stepIncrement: 0f,
                    "Minimum thrust percent required to auto-stage"));
             // --- Classic orbit target: inclination ---
            AddSuffix(new[] { "DESIREDINCLINATION", "INC" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetDesiredInclinationDouble(_ascentSettings),
                    value => SetDesiredInclination(_ascentSettings, value),
                    "Desired orbital inclination (degrees)"));

            // --- Corrective steering ---
            AddSuffix(new[] { "CORRECTIVESTEERING", "CSTEER" },
                new SetSuffix<BooleanValue>(
                    () => GetCorrectiveSteering(_ascentSettings),
                    value => SetCorrectiveSteering(_ascentSettings, value),
                    "Enable classic corrective steering"));

            AddSuffix(new[] { "CORRECTIVESTEERINGGAIN", "CSTEERGAIN" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetCorrectiveSteeringGainDouble(_ascentSettings),
                    value => SetCorrectiveSteeringGain(_ascentSettings, value),
                    "Corrective steering gain"));

            // --- Roll profile ---
            AddSuffix(new[] { "FORCEROLL", "FROLL" },
                new SetSuffix<BooleanValue>(
                    () => GetForceRoll(_ascentSettings),
                    value => SetForceRoll(_ascentSettings, value),
                    "Force roll during ascent"));

            AddSuffix(new[] { "VERTICALROLL", "VROLL" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetVerticalRollDouble(_ascentSettings),
                    value => SetVerticalRoll(_ascentSettings, value),
                    "Roll angle during vertical ascent (degrees)"));

            AddSuffix(new[] { "TURNROLL", "TROLL" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetTurnRollDouble(_ascentSettings),
                    value => SetTurnRoll(_ascentSettings, value),
                    "Roll angle during gravity turn (degrees)"));

            AddSuffix(new[] { "ROLLALTITUDE", "ROLLALT" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetRollAltitudeDouble(_ascentSettings),
                    value => SetRollAltitude(_ascentSettings, value),
                    "Altitude at which roll is applied"));

            // --- AoA & Q limits ---
            AddSuffix(new[] { "LIMITAOA", "LIMAOA" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitAoA(_ascentSettings),
                    value => SetLimitAoA(_ascentSettings, value),
                    "Enable angle-of-attack limit"));

            AddSuffix(new[] { "MAXAOA", "MAXAOA" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetMaxAoADouble(_ascentSettings),
                    value => SetMaxAoA(_ascentSettings, value),
                    "Maximum angle of attack (degrees)"));

            AddSuffix(new[] { "AOALIMITFADEOUTPRESSURE", "AOAFADEQ" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetAoALimitFadeoutPressureDouble(_ascentSettings),
                    value => SetAoALimitFadeoutPressure(_ascentSettings, value),
                    "Pressure at which AoA limit fades out"));

            AddSuffix(new[] { "LIMITQA", "LIMQA" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetLimitQaDouble(_thrustController),
                    value => SetLimitQa(_thrustController, value),
                    "Dynamic pressure limit (Q)"));

            AddSuffix(new[] { "LIMITQAENABLED", "LIMQAEN" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitQaEnabled(_thrustController),
                    value => SetLimitQaEnabled(_thrustController, value),
                    "Enable dynamic pressure limit (Q)"));
            
            // --- Thrust controller safety limits ---
            AddSuffix(new[] { "LIMITTOPREVENTOVERHEATS", "LIMOVHT" },
                new SetSuffix<BooleanValue>(
                    () => GetLimitToPreventOverheats(_thrustController),
                    value => SetLimitToPreventOverheats(_thrustController, value),
                    "Limit thrust to prevent engine overheating"));

            // --- Node executor auto-warp ---
            AddSuffix(new[] { "AUTOWARP", "AWARP" },
                new SetSuffix<BooleanValue>(
                    () => GetAutoWarp(_nodeExecutor),
                    value => SetAutoWarp(_nodeExecutor, value),
                    "Enable automatic time warp for maneuver execution"));
        }

        public BooleanValue Enabled
        {
            get =>
                Initialized
                    ? new BooleanValue(GetEnabled(CoreInstance))
                    : throw new KOSException("Cannot get Enabled property of not initialized MechJebAscentWrapper");
            set
            {
                if (Initialized) SetEnabled(CoreInstance, value);
            }
        }


        public int AscentType => GetAscentTypeInteger(_ascentSettings);

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

        private MemberBinder Member(object target, string name)
            => new MemberBinder(target, name);

        private class MemberBinder
        {
            private readonly object _target;
            private readonly string _name;

            public MemberBinder(object target, string name)
            {
                _target = target;
                _name = name;
            }

            public Func<object, T> GetField<T>()
                => Reflect.On(_target).Field(_name).AsGetter<T>();

            public Action<object, T> SetField<T>()
                => Reflect.On(_target).Field(_name).AsSetter<T>();

            public Func<object, T> GetProp<T>()
                => Reflect.On(_target).Property(_name).AsGetter<T>();

            public Action<object, T> SetProp<T>()
                => Reflect.On(_target).Property(_name).AsSetter<T>();
        }
    }
}