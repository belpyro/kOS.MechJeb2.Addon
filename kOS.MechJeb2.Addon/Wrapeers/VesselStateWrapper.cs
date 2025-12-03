using System;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Utilities;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("VesselStateWrapper")]
    public class VesselStateWrapper : BaseWrapper, IVesselStateWrapper
    {
        private Func<object, object> _instanceGetter;

        protected override void BindObject()
        {
            _instanceGetter = Reflect.On(MasterMechJeb).Field("VesselState").AsGetter<object>();
            var coreType = _instanceGetter(MasterMechJeb).GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            var members = coreType.GetMembers(flags)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                .Where(m => m.HasAttributeNamed("ValueInfoItemAttribute"))
                .ToArray();

            foreach (var member in members)
            {
                var wrapperProp = GetType().GetProperty(member.Name, flags);
                if (wrapperProp == null)
                    continue;

                if (wrapperProp.PropertyType != typeof(Func<object, double>)) continue;
                var del = member.BuildDoubleGetter();
                wrapperProp.SetValue(this, del, null);
            }
        }

        private object VesselInfo => _instanceGetter(MasterMechJeb);

        protected override void InitializeSuffixes()
        {
            var getInfo = new Func<object>(() => VesselInfo);

            // Time & gravity
            AddSuffix(new[] { "TIME", "T" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Time(getInfo())),
                    "Universal time in seconds"));

            AddSuffix(new[] { "LOCALG", "G" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(LocalG(getInfo())),
                    "Local gravitational acceleration in m/s^2"));

            // Velocities
            AddSuffix(new[] { "SPEEDORBITAL", "ORBVEL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedOrbital(getInfo())),
                    "Orbital speed relative to the reference body (m/s)"));

            AddSuffix(new[] { "SPEEDSURFACE", "SURFVEL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedSurface(getInfo())),
                    "Surface speed relative to the rotating body (m/s)"));

            AddSuffix(new[] { "SPEEDVERTICAL", "VERTVEL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedVertical(getInfo())),
                    "Vertical component of surface velocity (m/s)"));

            AddSuffix(new[] { "SPEEDSURFACEHORIZONTAL", "SURFHVEL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedSurfaceHorizontal(getInfo())),
                    "Horizontal component of surface velocity (m/s)"));

            AddSuffix(new[] { "SPEEDORBITHORIZONTAL", "ORBHVEL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedOrbitHorizontal(getInfo())),
                    "Horizontal component of orbital velocity (m/s)"));

            // Attitude
            AddSuffix(new[] { "VESSELHEADING", "HDG" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(VesselHeading(getInfo())),
                    "Vessel heading in degrees (0 = North)"));

            AddSuffix(new[] { "VESSELPITCH", "PITCH" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(VesselPitch(getInfo())),
                    "Vessel pitch in degrees (0 = horizon, + up)"));

            AddSuffix(new[] { "VESSELROLL", "ROLL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(VesselRoll(getInfo())),
                    "Vessel roll in degrees"));

            // Altitudes
            AddSuffix(new[] { "ALTITUDEASL", "ALTASL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AltitudeAsl(getInfo())),
                    "Altitude above sea level (m)"));

            AddSuffix(new[] { "ALTITUDETRUE", "ALTTRUE" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AltitudeTrue(getInfo())),
                    "True altitude above terrain (m)"));

            AddSuffix(new[] { "SURFACEALTITUDEASL", "SURFALT" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SurfaceAltitudeAsl(getInfo())),
                    "Surface altitude above sea level under the vessel (m)"));

            // Orbit
            AddSuffix(new[] { "ORBITAPA", "APA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitApA(getInfo())),
                    "Apoapsis altitude above sea level (m)"));

            AddSuffix(new[] { "ORBITPEA", "PEA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitPeA(getInfo())),
                    "Periapsis altitude above sea level (m)"));

            AddSuffix(new[] { "ORBITPERIOD", "ORBITPER" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitPeriod(getInfo())),
                    "Orbital period (s)"));

            AddSuffix(new[] { "ORBITTIMETOAP", "TTOAP" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitTimeToAp(getInfo())),
                    "Time to apoapsis (s)"));

            AddSuffix(new[] { "ORBITTIMETOPE", "TTOPE" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitTimeToPe(getInfo())),
                    "Time to periapsis (s)"));

            AddSuffix(new[] { "ORBITLAN", "LAN" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitLan(getInfo())),
                    "Longitude of ascending node (deg)"));

            AddSuffix(new[] { "ORBITARGUMENTOFPERIAPSIS", "ARGPE" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitArgumentOfPeriapsis(getInfo())),
                    "Argument of periapsis (deg)"));

            AddSuffix(new[] { "ORBITINCLINATION", "INCL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitInclination(getInfo())),
                    "Orbital inclination (deg)"));

            AddSuffix(new[] { "ORBITECCENTRICITY", "ECC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitEccentricity(getInfo())),
                    "Orbital eccentricity"));

            AddSuffix(new[] { "ORBITSEMIAXIS", "SMA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitSemiMajorAxis(getInfo())),
                    "Orbital semi-major axis (m)"));

            // Surface position
            AddSuffix(new[] { "CELESTIALLONGITUDE", "CESTLON" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(CelestialLongitude(getInfo())),
                    "Sub-vessel longitude on the celestial body (deg)"));

            AddSuffix(new[] { "LATITUDE", "LAT" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Latitude(getInfo())),
                    "Vessel latitude (deg)"));

            AddSuffix(new[] { "LONGITUDE", "LON" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Longitude(getInfo())),
                    "Vessel longitude (deg)"));

            // Aero angles
            AddSuffix(new[] { "AOA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Aoa(getInfo())),
                    "Angle of attack relative to airflow (deg)"));

            AddSuffix(new[] { "AOS" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Aos(getInfo())),
                    "Angle of sideslip relative to airflow (deg)"));

            AddSuffix(new[] { "DISPLACEMENTANGLE", "DISPANG" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(DisplacementAngle(getInfo())),
                    "Angle between velocity vector and reference orientation (deg)"));

            // Aero / speed / drag
            AddSuffix(new[] { "MACH" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Mach(getInfo())),
                    "Mach number (speed relative to speed of sound)"));

            AddSuffix(new[] { "SPEEDOFSOUND", "SOS" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SpeedOfSound(getInfo())),
                    "Local speed of sound (m/s)"));

            AddSuffix(new[] { "DRAGCOEF", "CD" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(DragCoef(getInfo())),
                    "Effective drag coefficient"));

            // Atmosphere / pressure
            AddSuffix(new[] { "ATMOSPHERICDENSITYGRAMS", "RHO" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AtmosphericDensityGrams(getInfo())),
                    "Atmospheric density (g/m^3)"));

            AddSuffix(new[] { "MAXDYNAMICPRESSURE", "QMAX" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MaxDynamicPressure(getInfo())),
                    "Maximum dynamic pressure experienced this flight (Pa)"));

            AddSuffix(new[] { "DYNAMICPRESSURE", "Q" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(DynamicPressure(getInfo())),
                    "Current dynamic pressure (Pa)"));

            // Intake air
            AddSuffix(new[] { "INTAKEAIR", "INTAKE" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(IntakeAir(getInfo())),
                    "Instantaneous intake air resource amount"));

            AddSuffix(new[] { "INTAKEAIRALLINTAKES", "INTAKEALL" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(IntakeAirAllIntakes(getInfo())),
                    "Total intake air supply from all intakes"));

            AddSuffix(new[] { "INTAKEAIRNEEDED", "INTAKENEED" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(IntakeAirNeeded(getInfo())),
                    "Intake air needed by engines at current throttle"));

            AddSuffix(new[] { "INTAKEAIRATMAX", "INTAKEMAX" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(IntakeAirAtMax(getInfo())),
                    "Intake air supply at maximum speed/flow"));

            // Misc orbital / aerothermal
            AddSuffix(new[] { "ANGLETOPROGRADE", "ANGPRO" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AngleToPrograde(getInfo())),
                    "Angle between vessel forward and velocity prograde vector (deg)"));

            AddSuffix(new[] { "FREEMOLECULARAEROTHERMALFLUX", "FMFLUX" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(FreeMolecularAerothermalFlux(getInfo())),
                    "Estimated free molecular aerothermal flux (W/m^2)"));

            // Net forces
            AddSuffix(new[] { "PUREDRAG", "DRAG" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(PureDrag(getInfo())),
                    "Net drag force magnitude (kN)"));

            AddSuffix(new[] { "PURELIFT", "LIFT" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(PureLift(getInfo())),
                    "Net lift force magnitude (kN)"));
        }

        public override string context() => nameof(VesselStateWrapper);

        public Func<object, double> Time { get; internal set; }
        public Func<object, double> LocalG { get; internal set; }
        public Func<object, double> SpeedOrbital { get; internal set; }
        public Func<object, double> SpeedSurface { get; internal set; }
        public Func<object, double> SpeedVertical { get; internal set; }
        public Func<object, double> SpeedSurfaceHorizontal { get; internal set; }
        public Func<object, double> SpeedOrbitHorizontal { get; internal set; }
        public Func<object, double> VesselHeading { get; internal set; }
        public Func<object, double> VesselPitch { get; internal set; }
        public Func<object, double> VesselRoll { get; internal set; }
        public Func<object, double> AltitudeAsl { get; internal set; }
        public Func<object, double> AltitudeTrue { get; internal set; }
        public Func<object, double> SurfaceAltitudeAsl { get; internal set; }
        public Func<object, double> OrbitApA { get; internal set; }
        public Func<object, double> OrbitPeA { get; internal set; }
        public Func<object, double> OrbitPeriod { get; internal set; }
        public Func<object, double> OrbitTimeToAp { get; internal set; }
        public Func<object, double> OrbitTimeToPe { get; internal set; }
        public Func<object, double> OrbitLan { get; internal set; }
        public Func<object, double> OrbitArgumentOfPeriapsis { get; internal set; }
        public Func<object, double> OrbitInclination { get; internal set; }
        public Func<object, double> OrbitEccentricity { get; internal set; }
        public Func<object, double> OrbitSemiMajorAxis { get; internal set; }
        public Func<object, double> CelestialLongitude { get; internal set; }
        public Func<object, double> Latitude { get; internal set; }
        public Func<object, double> Longitude { get; internal set; }
        public Func<object, double> Aoa { get; internal set; }
        public Func<object, double> Aos { get; internal set; }
        public Func<object, double> DisplacementAngle { get; internal set; }
        public Func<object, double> Mach { get; internal set; }
        public Func<object, double> SpeedOfSound { get; internal set; }
        public Func<object, double> DragCoef { get; internal set; }
        public Func<object, double> AtmosphericDensityGrams { get; internal set; }
        public Func<object, double> MaxDynamicPressure { get; internal set; }
        public Func<object, double> DynamicPressure { get; internal set; }
        public Func<object, double> IntakeAir { get; internal set; }
        public Func<object, double> IntakeAirAllIntakes { get; internal set; }
        public Func<object, double> IntakeAirNeeded { get; internal set; }
        public Func<object, double> IntakeAirAtMax { get; internal set; }
        public Func<object, double> AngleToPrograde { get; internal set; }
        public Func<object, double> FreeMolecularAerothermalFlux { get; internal set; }
        public Func<object, double> PureDrag { get; internal set; }
        public Func<object, double> PureLift { get; internal set; }
    }
}