using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("StageStatsWrapper")]
    public class MechJebStageStatsWrapper : BaseWrapper
    {
        private Func<object, object> _atmoStatsGetter;
        private Func<object, object> _vacStatsGetter;
        private Action<object, object> _requestUpdate;

        private Func<object, double> _atmoTGetter;
        private Func<object, double> _vacTGetter;
        private Func<object, object> _atmoRGetter;
        private Func<object, object> _vacRGetter;
        private Func<object, object> _atmoVGetter;
        private Func<object, object> _vacVGetter;
        private Func<object, object> _atmoUGetter;
        private Func<object, object> _vacUGetter;

        private Func<object, double> _v3X;
        private Func<object, double> _v3Y;
        private Func<object, double> _v3Z;

        internal Type FuelStatsType { get; private set; }
        internal IReadOnlyList<FuelStatsAccessor> Accessors { get; private set; }

        private object Module => MasterMechJeb.GetComputedModule("MechJebModuleStageStats");

        protected override void BindObject()
        {
            var module = Module;
            var moduleType = module.GetType();

            _atmoStatsGetter = Reflect.OnType(moduleType).Field("AtmoStats").AsGetter<object>();
            _vacStatsGetter = Reflect.OnType(moduleType).Field("VacStats").AsGetter<object>();
            _requestUpdate = Reflect.OnType(moduleType).Method("RequestUpdate").AsAction();

            _atmoTGetter = Reflect.OnType(moduleType).Field("AtmoT").AsGetter<double>();
            _vacTGetter = Reflect.OnType(moduleType).Field("VacT").AsGetter<double>();
            _atmoRGetter = Reflect.OnType(moduleType).Field("AtmoR").AsGetter<object>();
            _vacRGetter = Reflect.OnType(moduleType).Field("VacR").AsGetter<object>();
            _atmoVGetter = Reflect.OnType(moduleType).Field("AtmoV").AsGetter<object>();
            _vacVGetter = Reflect.OnType(moduleType).Field("VacV").AsGetter<object>();
            _atmoUGetter = Reflect.OnType(moduleType).Field("AtmoU").AsGetter<object>();
            _vacUGetter = Reflect.OnType(moduleType).Field("VacU").AsGetter<object>();

            var v3Type = moduleType.GetField("VacR").FieldType;
            _v3X = Reflect.OnType(v3Type).Field("x").AsGetter<double>();
            _v3Y = Reflect.OnType(v3Type).Field("y").AsGetter<double>();
            _v3Z = Reflect.OnType(v3Type).Field("z").AsGetter<double>();

            var atmoField = moduleType.GetField("AtmoStats");
            FuelStatsType = atmoField.FieldType.GetGenericArguments()[0];

            Accessors = BuildAccessors(FuelStatsType);
        }

        protected override void InitializeSuffixes()
        {
            AddSuffix("STAGEDELTAV",
                new OneArgsSuffix<FuelStatsPairValue, ScalarValue>(
                    idx => new FuelStatsPairValue(this, (int)idx),
                    "Returns vacuum/atmosphere FuelStats for the given fuel-flow stage index"));

            AddSuffix("STAGECOUNT", new NoArgsSuffix<ScalarIntValue>(
                () =>
                {
                    RequestUpdate();
                    var vac = (IList)_vacStatsGetter(Module);
                    var atmo = (IList)_atmoStatsGetter(Module);
                    return new ScalarIntValue(Math.Min(vac?.Count ?? 0, atmo?.Count ?? 0));
                },
                "Number of stages valid for STAGEDELTAV indexing (min of vac/atmo segment counts)"));

            AddSuffix("VACT", new NoArgsSuffix<ScalarDoubleValue>(
                () => { RequestUpdate(); return new ScalarDoubleValue(_vacTGetter(Module)); },
                "Universal time of the last completed vacuum stage-stats simulation"));
            AddSuffix("ATMOT", new NoArgsSuffix<ScalarDoubleValue>(
                () => { RequestUpdate(); return new ScalarDoubleValue(_atmoTGetter(Module)); },
                "Universal time of the last completed atmospheric stage-stats simulation"));

            AddSuffix("VACR", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_vacRGetter(Module)); },
                "Position snapshot used for the last vacuum simulation"));
            AddSuffix("ATMOR", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_atmoRGetter(Module)); },
                "Position snapshot used for the last atmospheric simulation"));
            AddSuffix("VACV", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_vacVGetter(Module)); },
                "Velocity snapshot used for the last vacuum simulation"));
            AddSuffix("ATMOV", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_atmoVGetter(Module)); },
                "Velocity snapshot used for the last atmospheric simulation"));
            AddSuffix("VACU", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_vacUGetter(Module)); },
                "Forward (up) snapshot used for the last vacuum simulation"));
            AddSuffix("ATMOU", new NoArgsSuffix<Vector>(
                () => { RequestUpdate(); return ToVector(_atmoUGetter(Module)); },
                "Forward (up) snapshot used for the last atmospheric simulation"));
        }

        private Vector ToVector(object v3)
        {
            // Match MechJeb's V3ToWorldRotated: swizzle .xzy then multiply by Planetarium rotation.
            var rotated = Planetarium.fetch.rotation * new Vector3d(_v3X(v3), _v3Z(v3), _v3Y(v3));
            return new Vector(rotated.x, rotated.y, rotated.z);
        }

        public override string context() => nameof(MechJebStageStatsWrapper);

        internal void RequestUpdate()
        {
            var module = Module;
            if (module == null) return;
            _requestUpdate(module, null);
        }

        internal object GetVacFuelStats(int stage)
        {
            RequestUpdate();
            return IndexOrThrow((IList)_vacStatsGetter(Module), stage, "VacStats");
        }

        internal object GetAtmoFuelStats(int stage)
        {
            RequestUpdate();
            return IndexOrThrow((IList)_atmoStatsGetter(Module), stage, "AtmoStats");
        }

        private static object IndexOrThrow(IList list, int stage, string label)
        {
            if (list == null || stage < 0 || stage >= list.Count)
                throw new KOSException(
                    $"StageStats {label} index {stage} out of range (count={list?.Count ?? 0})");
            return list[stage];
        }

        private static List<FuelStatsAccessor> BuildAccessors(Type fuelStatsType)
        {
            var list = new List<FuelStatsAccessor>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var field in fuelStatsType.GetFields(flags))
                TryAdd(list, field, field.Name, field.FieldType);

            foreach (var prop in fuelStatsType.GetProperties(flags))
            {
                if (prop.GetMethod == null) continue;
                if (prop.GetIndexParameters().Length != 0) continue;
                TryAdd(list, prop, prop.Name, prop.PropertyType);
            }

            return list;
        }

        private static void TryAdd(List<FuelStatsAccessor> list, MemberInfo member, string name, Type valueType)
        {
            if (!IsNumeric(valueType)) return;
            list.Add(new FuelStatsAccessor(name.ToUpperInvariant(), member));
        }

        private static bool IsNumeric(Type t) =>
            t == typeof(double) || t == typeof(float) || t == typeof(decimal) ||
            t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte) ||
            t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort) || t == typeof(sbyte);
    }

    internal class FuelStatsAccessor
    {
        public string Name { get; }
        public Func<object, double> Getter { get; }

        public FuelStatsAccessor(string name, MemberInfo member)
        {
            Name = name;
            Getter = BuildDoubleGetter(member);
        }

        private static Func<object, double> BuildDoubleGetter(MemberInfo member)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var declaringType = member.DeclaringType ?? throw new InvalidOperationException();
            var castObj = Expression.Convert(objParam, declaringType);

            Expression valueExpr;
            Type valueType;
            switch (member)
            {
                case FieldInfo f:
                    valueExpr = Expression.Field(castObj, f);
                    valueType = f.FieldType;
                    break;
                case PropertyInfo p:
                    valueExpr = Expression.Property(castObj, p);
                    valueType = p.PropertyType;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported member kind");
            }

            if (valueType != typeof(double))
                valueExpr = Expression.Convert(valueExpr, typeof(double));

            return Expression.Lambda<Func<object, double>>(valueExpr, objParam).Compile();
        }
    }
}
