using System;
using kOS.Safe.Encapsulation;

namespace kOS.MechJeb.Addon.Core
{
    public interface IVesselStateWrapper : IBaseWrapper, ISuffixed
    {
        // Time and gravity
        Func<object, double> Time { get; }
        Func<object, double> LocalG { get; }

        // Speeds
        Func<object, double> SpeedOrbital { get; }
        Func<object, double> SpeedSurface { get; }
        Func<object, double> SpeedVertical { get; }
        Func<object, double> SpeedSurfaceHorizontal { get; }
        Func<object, double> SpeedOrbitHorizontal { get; }

        // Oriented
        Func<object, double> VesselHeading { get; }
        Func<object, double> VesselPitch { get; }
        Func<object, double> VesselRoll { get; }

        // Altitudes
        Func<object, double> AltitudeAsl { get; }
        Func<object, double> AltitudeTrue { get; }
        Func<object, double> SurfaceAltitudeAsl { get; }

        // Orbit
        Func<object, double> OrbitApA { get; }
        Func<object, double> OrbitPeA { get; }
        Func<object, double> OrbitPeriod { get; }
        Func<object, double> OrbitTimeToAp { get; }
        Func<object, double> OrbitTimeToPe { get; }
        Func<object, double> OrbitLan { get; }
        Func<object, double> OrbitArgumentOfPeriapsis { get; }
        Func<object, double> OrbitInclination { get; }
        Func<object, double> OrbitEccentricity { get; }
        Func<object, double> OrbitSemiMajorAxis { get; }

        // Surface position
        Func<object, double> CelestialLongitude { get; }
        Func<object, double> Latitude { get; }
        Func<object, double> Longitude { get; }

        // Aerodynamic angles
        Func<object, double> Aoa { get; }
        Func<object, double> Aos { get; }
        Func<object, double> DisplacementAngle { get; }

        // Aerodynamic
        Func<object, double> Mach { get; }
        Func<object, double> SpeedOfSound { get; }
        Func<object, double> DragCoef { get; }

        // Atmo
        Func<object, double> AtmosphericDensityGrams { get; }
        Func<object, double> MaxDynamicPressure { get; }
        Func<object, double> DynamicPressure { get; }

        // Air
        Func<object, double> IntakeAir { get; }
        Func<object, double> IntakeAirAllIntakes { get; }
        Func<object, double> IntakeAirNeeded { get; }
        Func<object, double> IntakeAirAtMax { get; }

        // Orbital
        Func<object, double> AngleToPrograde { get; }
        Func<object, double> FreeMolecularAerothermalFlux { get; }

        // Drag/Lift
        Func<object, double> PureDrag { get; }
        Func<object, double> PureLift { get; }
    }
}