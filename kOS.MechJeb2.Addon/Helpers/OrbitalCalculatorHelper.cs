using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.KOSextensions;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;
using kOS.Suffixed;

namespace kOS.MechJeb2.Addon.Helpers
{
    [KOSNomenclature("OrbitalManeuverCalculator")]
    public class OrbitalCalculatorHelper : Structure, IOrbitalManeuverCalculator
    {
        private sealed class MethodOverload
        {
            public MethodInfo Method { get; set; }
            public Func<object[], object> Invoker { get; set; }

            // Input parameter types – only those actually passed through object[] args
            public Type[] InputTypes { get; set; }
        }

        private static Dictionary<string, List<MethodOverload>> _methodCache;
        private static OrbitalCalculatorHelper _instance;
        private static readonly string _typeName = "MuMech.OrbitalManeuverCalculator";
        private static Type _type;
        private static bool _typeInitialized = _type != null;

        public OrbitalCalculatorHelper()
        {
            InitializeCache();
            RegisterInitializer(InitializeSuffixes);
        }

        public static OrbitalCalculatorHelper Instance => _instance ??= new OrbitalCalculatorHelper();

        private void InitializeCache()
        {
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
            {
                if (_typeInitialized || !t.FullName!.Equals(_typeName, StringComparison.OrdinalIgnoreCase)) return;
                _type = t;
                _typeInitialized = true;
            });
            if (!_typeInitialized) return;

            _methodCache = _type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .GroupBy(m => m.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(m =>
                    {
                        var paramInfos = m.GetParameters();

                        // External parameters = all except out
                        var inputTypes = paramInfos
                            .Where(p => !p.IsOut)
                            .Select(p =>
                            {
                                var t = p.ParameterType;
                                // ref T → take T
                                return t.IsByRef ? t.GetElementType()! : t;
                            })
                            .ToArray();

                        return new MethodOverload
                        {
                            Method = m,
                            Invoker = m.MakeStaticMethodWithArgs(),
                            InputTypes = inputTypes
                        };
                    }).ToList()
                );
        }

        private void InitializeSuffixes()
        {
            AddSuffix(new[] { "DELTAVTOCIRCULARIZE", "DV2CIRC" },
                new TwoArgsSuffix<Vector, Orbitable, ScalarDoubleValue>((o, ut) => DeltaVToCircularize(o.Orbit, ut),
                    "Computes the deltaV of the burn needed to circularize an orbit at a given UT."));
            AddSuffix(new[] { "DELTAVTOELLIPTICIZE", "DV2ELL" },
                new FourArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue, ScalarDoubleValue>((o, ut,
                        newPe, newAp) => DeltaVToEllipticize(o.Orbit, ut, newPe, newAp)
                    , "Computes the deltaV of the burn needed to set a given PeR and ApR at at a given UT"));
            AddSuffix(new[] { "DELTAVTOCHANGEPERIAPSIS", "DV2PE" },
                new ThreeArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, ut, newPe) => DeltaVToChangePeriapsis(o.Orbit, ut, newPe))));
            AddSuffix(new[] { "DELTAVTOCHANGEAPOAPSIS", "DV2APO" },
                new ThreeArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, ut, newApo) => DeltaVToChangeApoapsis(o.Orbit, ut, newApo))));
            AddSuffix(new[] { "DELTAVTOCHANGEECCENTRICITY", "DV2ECC" },
                new ThreeArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, ut, newEcc) => DeltaVToChangeEccentricity(o.Orbit, ut, newEcc))));
            AddSuffix(new[] { "DELTAVFORSEMIMAJORAXIS", "DV4SMJAX" },
                new ThreeArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, ut, newSMA) => DeltaVForSemiMajorAxis(o.Orbit, ut, newSMA))));
            AddSuffix(new[] { "DELTAVTOCHANGEINCLINATION", "DV2INCL" },
                new ThreeArgsSuffix<Vector, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, ut, newInclination) => DeltaVToChangeInclination(o.Orbit, ut, newInclination))));
            AddSuffix(new[] { "HEADINGFORLAUNCHINCLINATION", "HEADING4LAUNCHINC" },
                new ThreeArgsSuffix<ScalarDoubleValue, Orbitable, ScalarDoubleValue, ScalarDoubleValue>(
                    ((o, inclinationDegrees, desiredApoapsis) =>
                        HeadingForLaunchInclination(o.Orbit, inclinationDegrees, desiredApoapsis))));
            AddSuffix(new[] { "DELTAVANDTIMETOMATCHPLANESASCENDING", "DV2MATCHPLANESASC" },
                new ThreeArgsSuffix<Lexicon, Orbitable, Orbitable, ScalarDoubleValue>(
                    ((o, target, ut) => DeltaVAndTimeToMatchPlanesAscending(o.Orbit, target.Orbit, ut))));
            AddSuffix(new[] { "DELTAVANDTIMETOMATCHPLANESDESCENDING", "DV2MATCHPLANESDESC" },
                new ThreeArgsSuffix<Lexicon, Orbitable, Orbitable, ScalarDoubleValue>(
                    ((o, target, ut) => DeltaVAndTimeToMatchPlanesDescending(o.Orbit, target.Orbit, ut))));
        }

        public Vector DeltaVToCircularize(Orbit o, double ut)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToCircularize), o, ut);
            return new Vector(result);
        }

        public Vector DeltaVToEllipticize(Orbit o, double ut, double newPeR, double newApR)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToEllipticize), o, ut, newPeR, newApR);
            return new Vector(result);
        }

        public Vector DeltaVToChangePeriapsis(Orbit o, double ut, double newPeR)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToChangePeriapsis), o, ut, newPeR);
            return new Vector(result);
        }

        public Vector DeltaVToChangeApoapsis(Orbit o, double ut, double newApR)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToChangeApoapsis), o, ut, newApR);
            return new Vector(result);
        }

        public Vector DeltaVToChangeEccentricity(Orbit o, double ut, double newEcc)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToChangeEccentricity), o, ut, newEcc);
            return new Vector(result);
        }

        public Vector DeltaVForSemiMajorAxis(Orbit o, double ut, double newSMA)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVForSemiMajorAxis), o, ut, newSMA);
            return new Vector(result);
        }

        public ScalarDoubleValue
            HeadingForLaunchInclination(Orbit o, double inclinationDegrees, double desiredApoapsis) =>
            InternalMethodExecute<double>(nameof(HeadingForLaunchInclination), o, inclinationDegrees, desiredApoapsis);

        public Vector DeltaVToChangeInclination(Orbit o, double ut, double newInclination)
        {
            var result = InternalMethodExecute<Vector3d>(nameof(DeltaVToChangeInclination), o, ut, newInclination);
            return new Vector(result);
        }

        public Lexicon DeltaVAndTimeToMatchPlanesAscending(Orbit o, Orbit target, double ut)
        {
            try
            {
                // returns Vector3d and an out double
                var result =
                    InternalMethodExecute<ValueTuple<Vector3d, double>>(nameof(DeltaVAndTimeToMatchPlanesAscending), o,
                        target,
                        ut);
                var lexicon = new Lexicon
                {
                    { new StringValue("DELTAV"), new Vector(result.Item1) },
                    { new StringValue("BURNUT"), new ScalarDoubleValue(result.Item2) }
                };
                return lexicon;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public Lexicon DeltaVAndTimeToMatchPlanesDescending(Orbit o, Orbit target, double ut)
        {
            try
            {
                // returns Vector3d and an out double
                var result =
                    InternalMethodExecute<ValueTuple<Vector3d, double>>(nameof(DeltaVAndTimeToMatchPlanesDescending), o,
                        target,
                        ut);
                var lexicon = new Lexicon
                {
                    { new StringValue("DELTAV"), new Vector(result.Item1) },
                    { new StringValue("BURNUT"), new ScalarDoubleValue(result.Item2) }
                };
                return lexicon;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public (Vector dV1, double UT1, Vector dV2, double UT2) DeltaVAndTimeForHohmannTransfer(Orbit o, Orbit target,
            double ut,
            double lagTime = Double.NaN, bool fixedTime = false, bool coplanar = true, bool rendezvous = true,
            bool capture = true)
        {
            throw new NotImplementedException();
        }

        public (Vector v1, Vector v2) DeltaVToInterceptAtTime(Orbit o, double t0, Orbit target, double dt,
            double offsetDistance = 0,
            bool shortway = true)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVAndTimeForCheapestCourseCorrection(Orbit o, double ut, Orbit target, out double burnUT)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVAndTimeForCheapestCourseCorrection(Orbit o, double ut, Orbit target,
            CelestialBody targetBody,
            double finalPeR, out double burnUT)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVAndTimeForCheapestCourseCorrection(Orbit o, double ut, Orbit target, double caDistance,
            out double burnUT)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVAndTimeForInterplanetaryTransferEjection(Orbit o, double ut, Orbit target,
            bool syncPhaseAngle,
            out double burnUT)
        {
            throw new NotImplementedException();
        }

        public (Vector dv, double dt) DeltaVAndTimeForMoonReturnEjection(Orbit o, double ut, double targetPrimaryRadius)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVToMatchVelocities(Orbit o, double ut, Orbit target)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVToResonantOrbit(Orbit o, double ut, double f)
        {
            throw new NotImplementedException();
        }

        public ScalarDoubleValue Distance(double lat_a, double long_a, double lat_b, double long_b)
        {
            throw new NotImplementedException();
        }

        public ScalarDoubleValue Heading(double lat_a, double long_a, double lat_b, double long_b)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVToShiftLAN(Orbit o, double ut, double newLAN)
        {
            throw new NotImplementedException();
        }

        public Vector DeltaVToShiftNodeLongitude(Orbit o, double ut, double newNodeLong)
        {
            throw new NotImplementedException();
        }

        public void PatchedConicInterceptBody(Orbit initial, CelestialBody target, Vector dV, double burnUT,
            double arrivalUT,
            out Orbit intercept)
        {
            throw new NotImplementedException();
        }

        public void SOI_intercept(Orbit transfer, CelestialBody target, double ut1, double ut2, out double ut)
        {
            throw new NotImplementedException();
        }

        private T InternalMethodExecute<T>(string name, params object[] args)
        {
            if (_methodCache == null || !_methodCache.TryGetValue(name, out var overloads))
                return default;

            foreach (var overload in overloads)
            {
                var expectedTypes = overload.InputTypes;

                if (expectedTypes.Length != args.Length)
                    continue;

                var match = true;

                for (int i = 0; i < expectedTypes.Length; i++)
                {
                    var expected = expectedTypes[i];
                    var arg = args[i];

                    if (arg == null)
                    {
                        // null ≠ non-nullable value type
                        if (expected.IsValueType && Nullable.GetUnderlyingType(expected) == null)
                        {
                            match = false;
                            break;
                        }
                    }
                    else if (!expected.IsInstanceOfType(arg))
                    {
                        // Could be extended using Convert.ChangeType / IsAssignableFrom, but this basic check is enough
                        match = false;
                        break;
                    }
                }

                if (!match)
                    continue;

                // Found matching overload → invoke it
                var result = overload.Invoker(args);
                return (T)result;
            }

            return default;
        }
    }
}