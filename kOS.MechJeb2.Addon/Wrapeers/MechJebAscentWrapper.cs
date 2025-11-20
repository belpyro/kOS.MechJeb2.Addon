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

        public override void Initialize(object coreInstance)
        {
            if (Initialized) return;
            base.Initialize(coreInstance);
            RegisterInitializer(InitializeSuffixes);
        }

        protected override void BindObject()
        {
            GetEnabled = Reflect.On(CoreInstance)
                .Property(nameof(Enabled))
                .AsGetter<bool>();
            SetEnabled = Reflect.On(CoreInstance)
                .Property(nameof(Enabled))
                .AsSetter<bool>();
            _coreModule = Reflect.On(CoreInstance)
                .Field("Core")
                .AsGetter<object>()(CoreInstance);
            _ascentSettings = Reflect.On(_coreModule)
                .Field("AscentSettings")
                .AsGetter<object>()(_coreModule);
            _stagingController = _coreModule.GetComputedModule("MechJebModuleStagingController");
            var desiredAltitudeField = Reflect.On(_ascentSettings)
                .Field("DesiredOrbitAltitude");
            GetDesiredAltitude = desiredAltitudeField
                .AsGetter<object>();
            GetDesiredAltitudeDouble = desiredAltitudeField.AsGetter<double>();
            SetDesiredAltitude = Reflect.On(GetDesiredAltitude(_ascentSettings))
                .Property("Val")
                .AsSetter<double>();
            GetAutoPath = Reflect.On(_ascentSettings)
                .Field("AutoPath")
                .AsGetter<bool>();
            SetAutoPath = Reflect.On(_ascentSettings)
                .Field("AutoPath")
                .AsSetter<bool>();
            GetAscentTypeInteger = Reflect.On(_ascentSettings)
                .Field("AscentTypeInteger")
                .AsGetter<int>();
            var turnStartAltitudeField = Reflect.On(_ascentSettings)
                .Field("TurnStartAltitude");
            GetTurnStartAltitude = turnStartAltitudeField
                .AsGetter<object>();
            GetTurnStartAltitudeDouble = turnStartAltitudeField.AsGetter<double>();
            SetTurnStartAltitude = Reflect.On(GetTurnStartAltitude(_ascentSettings))
                .Property("Val")
                .AsSetter<double>();
            GetAutostage = Reflect.On(_ascentSettings)
                .Property("Autostage")
                .AsGetter<bool>();
            SetAutostage = Reflect.On(_ascentSettings)
                .Property("Autostage")
                .AsSetter<bool>();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("ENABLED",
                new SetSuffix<BooleanValue>(() => Enabled, value => Enabled = value, "Is Ascent autopilot enable?"));
            AddSuffix(new[] { "DESIREDALTITUDE", "DESIREDALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetDesiredAltitudeDouble(_ascentSettings),
                    value => SetDesiredAltitude(GetDesiredAltitude(_ascentSettings), value),
                    "Desired Altitude"));
            AddSuffix(new[] { "TURNSTARTALTITUDE", "TURNSTARTALT" },
                new SetSuffix<ScalarDoubleValue>(() => GetTurnStartAltitudeDouble(_ascentSettings),
                    value => SetTurnStartAltitude(GetTurnStartAltitude(_ascentSettings), value),
                    "Turn Start Altitude"));
            AddSuffix(new[] { "AUTOPATH" },
                new SetSuffix<BooleanValue>(() => GetAutoPath(_ascentSettings),
                    value => SetAutoPath(_ascentSettings, value), "Automatic Altitude Turn"));
            AddSuffix(new[] { "AUTOSTAGE" },
                new SetSuffix<BooleanValue>(() => GetAutostage(_ascentSettings),
                    value => SetAutostage(_ascentSettings, value), "Auto Stage Status"));
            AddSuffix(new [] { "ASCENTTYPE" }, new NoArgsSuffix<StringValue>(() => AscentType == 0 ? "CLASSIC" : "NOT SUPPORTED"));
        }

        public BooleanValue Enabled
        {
            get
            {
                if (Initialized) return new BooleanValue(GetEnabled(CoreInstance));
                throw new KOSException("Cannot get Enabled property of not initialized MechJebAscentWrapper");
            }
            set
            {
                if (Initialized) SetEnabled(CoreInstance, value);
            }
        }

        public int AscentType => GetAscentTypeInteger(_ascentSettings);

        private Func<object, bool> GetEnabled { get; set; }
        private Action<object, bool> SetEnabled { get; set; }

        private Func<object, object> GetDesiredAltitude { get; set; }
        private Func<object, double> GetDesiredAltitudeDouble { get; set; }

        private Action<object, double> SetDesiredAltitude { get; set; }

        private Func<object, bool> GetAutoPath { get; set; }
        private Action<object, bool> SetAutoPath { get; set; }

        private Func<object, int> GetAscentTypeInteger { get; set; }
        
        //Autostage
        private Func<object,bool> GetAutostage { get; set; }
        private Action<object,bool> SetAutostage { get; set; }
        
        //TurnStartAltitude EditableDoubleMult
        private Func<object,object> GetTurnStartAltitude { get; set; }
        private Func<object,double> GetTurnStartAltitudeDouble { get; set; }
        private Action<object,double> SetTurnStartAltitude { get; set; }
        //TurnStartVelocity EditableDoubleMult
        //TurnEndAltitude 
        //TurnEndAngle
        //TurnShapeExponent
    }
}