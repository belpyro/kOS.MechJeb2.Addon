using System;

namespace kOS.MechJeb.Addon.Core
{
    public interface IMechJebInfoItemsWrapper : IBaseWrapper
    {
        // Maneuver node / orbit misc
        Func<object, string> NextManeuverNodeBurnTime { get; }
        Func<object, string> TimeToManeuverNode { get; }
        Func<object, string> NextManeuverNodeDeltaV { get; }

        // TWR / thrust / acceleration
        Func<object, double> SurfaceTWR { get; }
        Func<object, double> LocalTWR { get; }
        Func<object, double> ThrottleTWR { get; }
        Func<object, double> CurrentAcceleration { get; }
        Func<object, double> CurrentThrust { get; }
        Func<object, double> MaxThrust { get; }
        Func<object, double> MinThrust { get; }
        Func<object, double> MaxAcceleration { get; }
        Func<object, double> MinAcceleration { get; }
        Func<object, double> Acceleration { get; }

        // Atmosphere / pressure / drag
        Func<object, double> AtmosphericPressurekPA { get; }
        Func<object, double> AtmosphericPressure { get; }
        Func<object, double> AtmosphericDrag { get; }
        Func<object, double> DragCoefficient { get; }

        // Coordinates / position
        Func<object, string> GetCoordinateString { get; }

        // Orbit basic
        Func<object, double> MeanAnomaly { get; }
        Func<object, string> CurrentOrbitSummary { get; }
        Func<object, string> TargetOrbitSummary { get; }
        Func<object, string> CurrentOrbitSummaryWithInclination { get; }
        Func<object, string> TargetOrbitSummaryWithInclination { get; }
        Func<object, double> OrbitalEnergy { get; }
        Func<object, double> PotentialEnergy { get; }
        Func<object, double> KineticEnergy { get; }
        Func<object, string> TimeToImpact { get; }
        Func<object, string> SuicideBurnCountdown { get; }
        Func<object, string> TimeToSOITransition { get; }
        Func<object, double> SurfaceGravity { get; }
        Func<object, double> EscapeVelocity { get; }
        Func<object, double> CircularOrbitSpeed { get; }

        // RCS
        Func<object, double> RCSThrust { get; }
        Func<object, string> RCSTranslationEfficiency { get; }
        Func<object, double> RCSDeltaVVacuum { get; }

        // Angular velocity
        Func<object, string> AngularVelocity { get; }

        // Vessel basic
        Func<object, string> VesselName { get; }
        Func<object, string> VesselType { get; }
        Func<object, double> VesselMass { get; }
        Func<object, string> MaximumVesselMass { get; }
        Func<object, double> DryMass { get; }
        Func<object, double> LiquidFuelAndOxidizerMass { get; }
        Func<object, double> MonoPropellantMass { get; }
        Func<object, double> TotalElectricCharge { get; }
        Func<object, int> PartCount { get; }
        Func<object, string> MaxPartCount { get; }
        Func<object, string> PartCountAndMaxPartCount { get; }
        Func<object, int> StrutCount { get; }
        Func<object, int> FuelLinesCount { get; }
        Func<object, double> VesselCost { get; }
        Func<object, int> CrewCount { get; }
        Func<object, int> CrewCapacity { get; }

        // Target distance / relative motion
        Func<object, string> TargetDistance { get; }
        Func<object, string> HeadingToTarget { get; }
        Func<object, string> TargetRelativeVelocity { get; }
        Func<object, string> TargetTimeToClosestApproach { get; }
        Func<object, string> TargetClosestApproachDistance { get; }
        Func<object, string> TargetClosestApproachRelativeVelocity { get; }

        // Target SoI / capture
        Func<object, string> PeriapsisInTargetSOI { get; }
        Func<object, string> TargetCaptureDV { get; }

        // Target orbit numbers
        Func<object, string> TargetApoapsis { get; }
        Func<object, string> TargetPeriapsis { get; }
        Func<object, string> TargetInclination { get; }
        Func<object, string> TargetOrbitPeriod { get; }
        Func<object, string> TargetOrbitSpeed { get; }
        Func<object, string> TargetOrbitTimeToAp { get; }
        Func<object, string> TargetOrbitTimeToPe { get; }
        Func<object, string> TargetLAN { get; }
        Func<object, string> TargetLDN { get; }
        Func<object, string> TargetTimeToAscendingNode { get; }
        Func<object, string> TargetTimeToDescendingNode { get; }
        Func<object, string> TargetAoP { get; }
        Func<object, string> TargetEccentricity { get; }
        Func<object, string> TargetSMA { get; }
        Func<object, string> TargetMeanAnomaly { get; }
        Func<object, string> TargetTrueLongitude { get; }

        // Target / relative geometry
        Func<object, string> SynodicPeriod { get; }
        Func<object, string> PhaseAngle { get; }
        Func<object, string> TargetPlanetPhaseAngle { get; }
        Func<object, string> RelativeInclinationToTarget { get; }
        Func<object, string> TimeToAscendingNodeWithTarget { get; }
        Func<object, string> TimeToDescendingNodeWithTarget { get; }
        Func<object, string> TimeToEquatorialAscendingNode { get; }
        Func<object, string> TimeToEquatorialDescendingNode { get; }

        // Biomes
        Func<object, string> CurrentRawBiome { get; }
        Func<object, string> CurrentBiome { get; }

        // Stage dV / time
        Func<object, double> StageDeltaVVacuum { get; }
        Func<object, double> StageDeltaVAtmosphere { get; }
        Func<object, string> StageDeltaVAtmosphereAndVac { get; }
        Func<object, float> StageTimeLeftFullThrottle { get; }
        Func<object, float> StageTimeLeftCurrentThrottle { get; }
        Func<object, float> StageTimeLeftHover { get; }
        Func<object, double> TotalDeltaVVaccum { get; }
        Func<object, double> TotalDeltaVAtmosphere { get; }
        Func<object, string> TotalDeltaVAtmosphereAndVac { get; }
    }
    }