using System;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("LandingAutopilotWrapper")]
    public class MechJebLandingAutopilotWrapper : BaseWrapper, IMechJebLandingAutopilotWrapper
    {
        private Func<object, object> _autopilotGetter;

        // Users pool for proper autopilot engagement (like MechJeb GUI does)
        private Func<object, object> _usersGetter;
        private Action<object, object> _usersAdd;
        private Action<object, object> _usersRemove;
        private readonly object _userIdentity = new object(); // Sentinel object to identify this wrapper as a user

        // Property getters/setters
        private Func<object, bool> GetEnabled;
        private Action<object, bool> SetEnabled;

        private Func<object, double> GetTouchdownSpeedDouble;
        private Action<object, double> SetTouchdownSpeed;

        private Func<object, bool> GetDeployGears;
        private Action<object, bool> SetDeployGears;

        private Func<object, int> GetLimitGearsStage;
        private Action<object, int> SetLimitGearsStage;

        private Func<object, bool> GetDeployChutes;
        private Action<object, bool> SetDeployChutes;

        private Func<object, int> GetLimitChutesStage;
        private Action<object, int> SetLimitChutesStage;

        private Func<object, bool> GetRCSAdjustment;
        private Action<object, bool> SetRCSAdjustment;

        private Func<object, bool> GetPredictionReady;
        private Func<object, object> GetLandingSite;

        // Landing action methods
        private Action<object, object> _landAtPositionTarget;
        private Action<object, object> _landUntargeted;
        private Action<object> _stopLanding;

        protected override void BindObject()
        {
            _autopilotGetter = Member(MasterMechJeb, "Landing").GetField<object>();
            var autopilot = _autopilotGetter(MasterMechJeb);

            GetEnabled = Member(autopilot, nameof(Enabled)).GetProp<bool>();
            SetEnabled = Member(autopilot, nameof(Enabled)).SetProp<bool>();

            // Bind to Users pool for proper autopilot engagement
            // MechJeb GUI uses _autopilot.Users.Add(this) to engage, not Enabled = true directly
            _usersGetter = Member(autopilot, "Users").GetField<object>();
            var users = _usersGetter(autopilot);
            _usersAdd = Reflect.OnType(users.GetType()).Method("Add").WithArgs(typeof(object)).AsAction();
            _usersRemove = Reflect.OnType(users.GetType()).Method("Remove").WithArgs(typeof(object)).AsAction();

            // Bind configuration properties
            (GetTouchdownSpeedDouble, SetTouchdownSpeed) =
                BindEditable<double>(autopilot, "TouchdownSpeed");

            GetDeployGears = Member(autopilot, "DeployGears").GetField<bool>();
            SetDeployGears = Member(autopilot, "DeployGears").SetField<bool>();

            (GetLimitGearsStage, SetLimitGearsStage) =
                BindEditable<int>(autopilot, "LimitGearsStage");

            GetDeployChutes = Member(autopilot, "DeployChutes").GetField<bool>();
            SetDeployChutes = Member(autopilot, "DeployChutes").SetField<bool>();

            (GetLimitChutesStage, SetLimitChutesStage) =
                BindEditable<int>(autopilot, "LimitChutesStage");

            GetRCSAdjustment = Member(autopilot, "RCSAdjustment").GetField<bool>();
            SetRCSAdjustment = Member(autopilot, "RCSAdjustment").SetField<bool>();

            // Status properties
            GetPredictionReady = Member(autopilot, "PredictionReady").GetProp<bool>();
            GetLandingSite = Member(autopilot, "LandingSite").GetProp<object>();

            // Landing action methods
            _landAtPositionTarget = Reflect.OnType(autopilot.GetType())
                .Method("LandAtPositionTarget")
                .WithArgs(typeof(object))
                .AsAction();

            _landUntargeted = Reflect.OnType(autopilot.GetType())
                .Method("LandUntargeted")
                .WithArgs(typeof(object))
                .AsAction();

            // StopLanding is a void method with no parameters
            var stopLandingMethod = autopilot.GetType().GetMethod("StopLanding");
            _stopLanding = inst => stopLandingMethod.Invoke(inst, new object[0]);
        }

        protected override void InitializeSuffixes()
        {
            // Enable/disable autopilot
            AddSuffix("ENABLED",
                new SetSuffix<BooleanValue>(() => Enabled, value => Enabled = value,
                    "Is Landing autopilot enabled?"));

            // Landing actions
            AddSuffix(new[] { "LANDATTARGET", "LANDTARGET" },
                new NoArgsSuffix<BooleanValue>(() =>
                {
                    if (!Initialized) throw new KOSException("Landing autopilot not initialized");
                    var autopilot = _autopilotGetter(MasterMechJeb);
                    _landAtPositionTarget(autopilot, _userIdentity);
                    return true;
                },
                "Engage landing autopilot to land at position target"));

            AddSuffix(new[] { "LANDUNTARGETED", "LANDANYWHERE" },
                new NoArgsSuffix<BooleanValue>(() =>
                {
                    if (!Initialized) throw new KOSException("Landing autopilot not initialized");
                    var autopilot = _autopilotGetter(MasterMechJeb);
                    _landUntargeted(autopilot, _userIdentity);
                    return true;
                },
                "Engage landing autopilot to land anywhere"));

            AddSuffix(new[] { "STOPLANDING", "ABORT" },
                new NoArgsSuffix<BooleanValue>(() =>
                {
                    if (!Initialized) throw new KOSException("Landing autopilot not initialized");
                    var autopilot = _autopilotGetter(MasterMechJeb);
                    _stopLanding(autopilot);
                    return true;
                },
                "Stop landing autopilot"));

            // Configuration properties
            AddSuffix(new[] { "TOUCHDOWNSPEED", "TDSPEED" },
                new SetSuffix<ScalarDoubleValue>(
                    () => GetTouchdownSpeedDouble(_autopilotGetter(MasterMechJeb)),
                    value => SetTouchdownSpeed(_autopilotGetter(MasterMechJeb), value),
                    "Target touchdown speed in m/s"));

            AddSuffix(new[] { "DEPLOYGEARS", "GEARS" },
                new SetSuffix<BooleanValue>(
                    () => GetDeployGears(_autopilotGetter(MasterMechJeb)),
                    value => SetDeployGears(_autopilotGetter(MasterMechJeb), value),
                    "Automatically deploy landing gear"));

            AddSuffix(new[] { "LIMITGEARSSTAGE", "GEARSLIMIT" },
                new SetSuffix<ScalarIntValue>(
                    () => GetLimitGearsStage(_autopilotGetter(MasterMechJeb)),
                    value => SetLimitGearsStage(_autopilotGetter(MasterMechJeb), (int)value),
                    "Stage limit for deploying landing gear"));

            AddSuffix(new[] { "DEPLOYCHUTES", "CHUTES" },
                new SetSuffix<BooleanValue>(
                    () => GetDeployChutes(_autopilotGetter(MasterMechJeb)),
                    value => SetDeployChutes(_autopilotGetter(MasterMechJeb), value),
                    "Automatically deploy parachutes"));

            AddSuffix(new[] { "LIMITCHUTESSTAGE", "CHUTESLIMIT" },
                new SetSuffix<ScalarIntValue>(
                    () => GetLimitChutesStage(_autopilotGetter(MasterMechJeb)),
                    value => SetLimitChutesStage(_autopilotGetter(MasterMechJeb), (int)value),
                    "Stage limit for deploying parachutes"));

            AddSuffix(new[] { "RCSADJUSTMENT", "RCS" },
                new SetSuffix<BooleanValue>(
                    () => GetRCSAdjustment(_autopilotGetter(MasterMechJeb)),
                    value => SetRCSAdjustment(_autopilotGetter(MasterMechJeb), value),
                    "Use RCS for fine adjustments during landing"));

            // Status properties (read-only)
            AddSuffix(new[] { "PREDICTIONREADY", "READY" },
                new NoArgsSuffix<BooleanValue>(
                    () => GetPredictionReady(_autopilotGetter(MasterMechJeb)),
                    "Is landing prediction ready?"));

            AddSuffix(new[] { "LANDINGSITE", "SITE" },
                new NoArgsSuffix<Vector>(
                    () =>
                    {
                        var site = GetLandingSite(_autopilotGetter(MasterMechJeb));
                        // site is a Vector3d, convert to kOS Vector
                        var x = (double)site.GetType().GetField("x").GetValue(site);
                        var y = (double)site.GetType().GetField("y").GetValue(site);
                        var z = (double)site.GetType().GetField("z").GetValue(site);
                        return new Vector(x, y, z);
                    },
                    "Predicted landing site (world position vector)"));
        }

        public BooleanValue Enabled
        {
            get =>
                Initialized
                    ? new BooleanValue(GetEnabled(_autopilotGetter(MasterMechJeb)))
                    : throw new KOSException("Cannot get Enabled property of not initialized MechJebLandingAutopilotWrapper");
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

        public override string context() => nameof(MechJebLandingAutopilotWrapper);

        // Helper methods for reflection bindings
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
