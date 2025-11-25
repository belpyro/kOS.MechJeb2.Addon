using System;
using System.Linq;
using System.Reflection;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("VesselStateWrapper"), Log]
    public class VesselStateWrapper : BaseWrapper, IVesselStateWrapper
    {
        protected override void BindObject()
        {
            var coreType = CoreInstance.GetType();
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

        protected override void InitializeSuffixes()
        {
            // Time & gravity
            AddSufixInternal("TIME", Time, "Universal time in seconds", "T");
            AddSufixInternal("LOCALG", LocalG, "Local gravitational acceleration in m/s^2", "G");

            // Velocities
            AddSufixInternal("SPEEDORBITAL", SpeedOrbital, "Orbital speed relative to the reference body (m/s)",
                "ORBVEL");
            AddSufixInternal("SPEEDSURFACE", SpeedSurface, "Surface speed relative to the rotating body (m/s)",
                "SURFVEL");
            AddSufixInternal("SPEEDVERTICAL", SpeedVertical, "Vertical component of surface velocity (m/s)",
                "VERTVEL");
            AddSufixInternal("SPEEDSURFACEHORIZONTAL", SpeedSurfaceHorizontal,
                "Horizontal component of surface velocity (m/s)",
                "SURFHVEL");
            AddSufixInternal("SPEEDORBITHORIZONTAL", SpeedOrbitHorizontal,
                "Horizontal component of orbital velocity (m/s)",
                "ORBHVEL");

            // Attitude
            AddSufixInternal("VESSELHEADING", VesselHeading, "Vessel heading in degrees (0 = North)", "HDG");
            AddSufixInternal("VESSELPITCH", VesselPitch, "Vessel pitch in degrees (0 = horizon, + up)", "PITCH");
            AddSufixInternal("VESSELROLL", VesselRoll, "Vessel roll in degrees", "ROLL");

            // Altitudes
            AddSufixInternal("ALTITUDEASL", AltitudeAsl, "Altitude above sea level (m)", "ALTASL");
            AddSufixInternal("ALTITUDETRUE", AltitudeTrue, "True altitude above terrain (m)", "ALTTRUE");
            AddSufixInternal("SURFACEALTITUDEASL", SurfaceAltitudeAsl,
                "Surface altitude above sea level under the vessel (m)",
                "SURFALT");

            // Orbit
            AddSufixInternal("ORBITAPA", OrbitApA, "Apoapsis altitude above sea level (m)", "APA");
            AddSufixInternal("ORBITPEA", OrbitPeA, "Periapsis altitude above sea level (m)", "PEA");
            AddSufixInternal("ORBITPERIOD", OrbitPeriod, "Orbital period (s)", "ORBITPER");
            AddSufixInternal("ORBITTIMETOAP", OrbitTimeToAp, "Time to apoapsis (s)", "TTOAP");
            AddSufixInternal("ORBITTIMETOPE", OrbitTimeToPe, "Time to periapsis (s)", "TTOPE");
            AddSufixInternal("ORBITLAN", OrbitLan, "Longitude of ascending node (deg)", "LAN");
            AddSufixInternal("ORBITARGUMENTOFPERIAPSIS", OrbitArgumentOfPeriapsis, "Argument of periapsis (deg)",
                "ARGPE");
            AddSufixInternal("ORBITINCLINATION", OrbitInclination, "Orbital inclination (deg)", "INCL");
            AddSufixInternal("ORBITECCENTRICITY", OrbitEccentricity, "Orbital eccentricity", "ECC");
            AddSufixInternal("ORBITSEMIAXIS", OrbitSemiMajorAxis, "Orbital semi-major axis (m)", "SMA");

            // Surface position
            AddSufixInternal("CELESTIALLONGITUDE", CelestialLongitude,
                "Sub-vessel longitude on the celestial body (deg)",
                "CESTLON");
            AddSufixInternal("LATITUDE", Latitude, "Vessel latitude (deg)", "LAT");
            AddSufixInternal("LONGITUDE", Longitude, "Vessel longitude (deg)", "LON");

            // Aero angles
            AddSufixInternal("AOA", Aoa, "Angle of attack relative to airflow (deg)");
            AddSufixInternal("AOS", Aos, "Angle of sideslip relative to airflow (deg)");
            AddSufixInternal("DISPLACEMENTANGLE", DisplacementAngle,
                "Angle between velocity vector and reference orientation (deg)", "DISPANG");

            // Aero / speed / drag
            AddSufixInternal("MACH", Mach, "Mach number (speed relative to speed of sound)", "MACH");
            AddSufixInternal("SPEEDOFSOUND", SpeedOfSound, "Local speed of sound (m/s)", "SOS");
            AddSufixInternal("DRAGCOEF", DragCoef, "Effective drag coefficient", "CD");

            // Atmosphere / pressure
            AddSufixInternal("ATMOSPHERICDENSITYGRAMS", AtmosphericDensityGrams, "Atmospheric density (g/m^3)", "RHO");
            AddSufixInternal("MAXDYNAMICPRESSURE", MaxDynamicPressure,
                "Maximum dynamic pressure experienced this flight (Pa)",
                "QMAX");
            AddSufixInternal("DYNAMICPRESSURE", DynamicPressure, "Current dynamic pressure (Pa)", "Q");

            // Intake air
            AddSufixInternal("INTAKEAIR", IntakeAir, "Instantaneous intake air resource amount", "INTAKE");
            AddSufixInternal("INTAKEAIRALLINTAKES", IntakeAirAllIntakes, "Total intake air supply from all intakes",
                "INTAKEALL");
            AddSufixInternal("INTAKEAIRNEEDED", IntakeAirNeeded, "Intake air needed by engines at current throttle",
                "INTAKENEED");
            AddSufixInternal("INTAKEAIRATMAX", IntakeAirAtMax, "Intake air supply at maximum speed/flow", "INTAKEMAX");

            // Misc orbital / aerothermal
            AddSufixInternal("ANGLETOPROGRADE", AngleToPrograde,
                "Angle between vessel forward and velocity prograde vector (deg)", "ANGPRO");
            AddSufixInternal("FREEMOLECULARAEROTHERMALFLUX", FreeMolecularAerothermalFlux,
                "Estimated free molecular aerothermal flux (W/m^2)", "FMFLUX");

            // Net forces
            AddSufixInternal("PUREDRAG", PureDrag, "Net drag force magnitude (kN)", "DRAG");
            AddSufixInternal("PURELIFT", PureLift, "Net lift force magnitude (kN)", "LIFT");
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