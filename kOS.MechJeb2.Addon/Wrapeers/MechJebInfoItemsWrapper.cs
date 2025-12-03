using System;
using System.Reflection;
using kOS.MechJeb2.Addon.Attributes;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Utilities;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("InfoWrapper")]
    public sealed class MechJebInfoItemsWrapper : BaseWrapper, IMechJebInfoItemsWrapper
    {
        // Maneuver node / orbit misc
        public Func<object, string> NextManeuverNodeBurnTime { get; internal set; }
        public Func<object, string> TimeToManeuverNode { get; internal set; }
        public Func<object, string> NextManeuverNodeDeltaV { get; internal set; }

        // TWR / thrust / acceleration
        public Func<object, double> SurfaceTWR { get; internal set; }
        public Func<object, double> LocalTWR { get; internal set; }
        public Func<object, double> ThrottleTWR { get; internal set; }
        public Func<object, double> CurrentAcceleration { get; internal set; }
        public Func<object, double> CurrentThrust { get; internal set; }
        public Func<object, double> MaxThrust { get; internal set; }
        public Func<object, double> MinThrust { get; internal set; }
        public Func<object, double> MaxAcceleration { get; internal set; }
        public Func<object, double> MinAcceleration { get; internal set; }
        public Func<object, double> Acceleration { get; internal set; }

        // Atmosphere / pressure / drag
        public Func<object, double> AtmosphericPressurekPA { get; internal set; }
        public Func<object, double> AtmosphericPressure { get; internal set; }
        public Func<object, double> AtmosphericDrag { get; internal set; }
        public Func<object, double> DragCoefficient { get; internal set; }

        // Coordinates / position
        public Func<object, string> GetCoordinateString { get; internal set; }

        // Orbit basic
        public Func<object, double> MeanAnomaly { get; internal set; }
        public Func<object, string> CurrentOrbitSummary { get; internal set; }
        public Func<object, string> TargetOrbitSummary { get; internal set; }
        public Func<object, string> CurrentOrbitSummaryWithInclination { get; internal set; }
        public Func<object, string> TargetOrbitSummaryWithInclination { get; internal set; }
        public Func<object, double> OrbitalEnergy { get; internal set; }
        public Func<object, double> PotentialEnergy { get; internal set; }
        public Func<object, double> KineticEnergy { get; internal set; }
        public Func<object, string> TimeToImpact { get; internal set; }
        public Func<object, string> SuicideBurnCountdown { get; internal set; }
        public Func<object, string> TimeToSOITransition { get; internal set; }
        public Func<object, double> SurfaceGravity { get; internal set; }
        public Func<object, double> EscapeVelocity { get; internal set; }
        public Func<object, double> CircularOrbitSpeed { get; internal set; }

        // RCS
        public Func<object, double> RCSThrust { get; internal set; }
        public Func<object, string> RCSTranslationEfficiency { get; internal set; }
        public Func<object, double> RCSDeltaVVacuum { get; internal set; }

        // Angular velocity
        public Func<object, string> AngularVelocity { get; internal set; }

        // Vessel basic
        public Func<object, string> VesselName { get; internal set; }
        public Func<object, string> VesselType { get; internal set; }
        public Func<object, double> VesselMass { get; internal set; }
        public Func<object, string> MaximumVesselMass { get; internal set; }
        public Func<object, double> DryMass { get; internal set; }
        public Func<object, double> LiquidFuelAndOxidizerMass { get; internal set; }
        public Func<object, double> MonoPropellantMass { get; internal set; }
        public Func<object, double> TotalElectricCharge { get; internal set; }
        public Func<object, int> PartCount { get; internal set; }
        public Func<object, string> MaxPartCount { get; internal set; }
        public Func<object, string> PartCountAndMaxPartCount { get; internal set; }
        public Func<object, int> StrutCount { get; internal set; }
        public Func<object, int> FuelLinesCount { get; internal set; }
        public Func<object, double> VesselCost { get; internal set; }
        public Func<object, int> CrewCount { get; internal set; }
        public Func<object, int> CrewCapacity { get; internal set; }

        // Target distance / relative motion
        public Func<object, string> TargetDistance { get; internal set; }
        public Func<object, string> HeadingToTarget { get; internal set; }
        public Func<object, string> TargetRelativeVelocity { get; internal set; }
        public Func<object, string> TargetTimeToClosestApproach { get; internal set; }
        public Func<object, string> TargetClosestApproachDistance { get; internal set; }
        public Func<object, string> TargetClosestApproachRelativeVelocity { get; internal set; }

        // Target SoI / capture
        public Func<object, string> PeriapsisInTargetSOI { get; internal set; }
        public Func<object, string> TargetCaptureDV { get; internal set; }

        // Target orbit numbers
        public Func<object, string> TargetApoapsis { get; internal set; }
        public Func<object, string> TargetPeriapsis { get; internal set; }
        public Func<object, string> TargetInclination { get; internal set; }
        public Func<object, string> TargetOrbitPeriod { get; internal set; }
        public Func<object, string> TargetOrbitSpeed { get; internal set; }
        public Func<object, string> TargetOrbitTimeToAp { get; internal set; }
        public Func<object, string> TargetOrbitTimeToPe { get; internal set; }
        public Func<object, string> TargetLAN { get; internal set; }
        public Func<object, string> TargetLDN { get; internal set; }
        public Func<object, string> TargetTimeToAscendingNode { get; internal set; }
        public Func<object, string> TargetTimeToDescendingNode { get; internal set; }
        public Func<object, string> TargetAoP { get; internal set; }
        public Func<object, string> TargetEccentricity { get; internal set; }
        public Func<object, string> TargetSMA { get; internal set; }
        public Func<object, string> TargetMeanAnomaly { get; internal set; }
        public Func<object, string> TargetTrueLongitude { get; internal set; }

        // Target / relative geometry
        public Func<object, string> SynodicPeriod { get; internal set; }
        public Func<object, string> PhaseAngle { get; internal set; }
        public Func<object, string> TargetPlanetPhaseAngle { get; internal set; }
        public Func<object, string> RelativeInclinationToTarget { get; internal set; }
        public Func<object, string> TimeToAscendingNodeWithTarget { get; internal set; }
        public Func<object, string> TimeToDescendingNodeWithTarget { get; internal set; }
        public Func<object, string> TimeToEquatorialAscendingNode { get; internal set; }
        public Func<object, string> TimeToEquatorialDescendingNode { get; internal set; }

        // Biomes
        public Func<object, string> CurrentRawBiome { get; internal set; }
        public Func<object, string> CurrentBiome { get; internal set; }

        // Stage dV / time
        public Func<object, double> StageDeltaVVacuum { get; internal set; }
        public Func<object, double> StageDeltaVAtmosphere { get; internal set; }
        public Func<object, string> StageDeltaVAtmosphereAndVac { get; internal set; }
        public Func<object, float> StageTimeLeftFullThrottle { get; internal set; }
        public Func<object, float> StageTimeLeftCurrentThrottle { get; internal set; }
        public Func<object, float> StageTimeLeftHover { get; internal set; }
        public Func<object, double> TotalDeltaVVaccum { get; internal set; }
        public Func<object, double> TotalDeltaVAtmosphere { get; internal set; }
        public Func<object, string> TotalDeltaVAtmosphereAndVac { get; internal set; }

        private static readonly BindingFlags _methodFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        private static readonly object _cacheLock = new object();

        private static readonly System.Collections.Generic.Dictionary<MethodInfo, Delegate> _methodCache =
            new System.Collections.Generic.Dictionary<MethodInfo, Delegate>();

        private object Module => MasterMechJeb.GetComputedModule("MechJebModuleInfoItems");

        protected override void BindObject()
        {
            BindInfoItems(Module.GetType());
        }

        protected override void InitializeSuffixes()
        {
            var getModule = new Func<object>(() => Module);

            // ===== Maneuver / Node =====
            AddSuffix(new[] { "NEXTMANEUVERNODEBURNTIME", "BURN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(NextManeuverNodeBurnTime(getModule())),
                    "Burn time for next maneuver node"));

            AddSuffix(new[] { "TIMETOMANEUVERNODE", "NODEETA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToManeuverNode(getModule())),
                    "Time to next maneuver node"));

            AddSuffix(new[] { "NEXTMANEUVERNODEDELTAV", "NODEDV" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(NextManeuverNodeDeltaV(getModule())),
                    "Node delta-V"));

            // ===== TWR / thrust / acceleration =====
            AddSuffix(new[] { "SURFACETWR", "STWR" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SurfaceTWR(getModule())),
                    "Surface TWR"));

            AddSuffix(new[] { "LOCALTWR", "LTWR" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(LocalTWR(getModule())),
                    "Local TWR"));

            AddSuffix(new[] { "THROTTLETWR", "TTWR" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(ThrottleTWR(getModule())),
                    "TWR at current throttle"));

            AddSuffix(new[] { "CURRENTACC", "ACC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(CurrentAcceleration(getModule())),
                    "Current acceleration"));

            AddSuffix(new[] { "CURRENTTHRUST", "THRUST" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(CurrentThrust(getModule())),
                    "Current thrust (kN)"));

            AddSuffix(new[] { "MAXTHRUST", "MAXTH" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MaxThrust(getModule())),
                    "Max possible thrust"));

            AddSuffix(new[] { "MINTHRUST", "MINTH" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MinThrust(getModule())),
                    "Minimum engine thrust"));

            AddSuffix(new[] { "MAXACC", "MAXACC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MaxAcceleration(getModule())),
                    "Max acceleration"));

            AddSuffix(new[] { "MINACC", "MINACC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MinAcceleration(getModule())),
                    "Minimum acceleration"));

            AddSuffix(new[] { "ACCELERATION", "A" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(Acceleration(getModule())),
                    "Net acceleration"));

            // ===== Atmosphere / drag =====
            AddSuffix(new[] { "ATMPRESSUREKPA", "P_KPA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AtmosphericPressurekPA(getModule())),
                    "Atmospheric pressure (kPa)"));

            AddSuffix(new[] { "ATMPRESSURE", "P" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AtmosphericPressure(getModule())),
                    "Atmospheric pressure"));

            AddSuffix(new[] { "ATMDESITYDRAG", "DRAG" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(AtmosphericDrag(getModule())),
                    "Drag force estimation"));

            AddSuffix(new[] { "DRAGCOEF", "CD" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(DragCoefficient(getModule())),
                    "Aerodynamic drag coefficient"));

            // ===== Coordinates / position =====
            AddSuffix(new[] { "COORDINATESTRING", "COORD" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(GetCoordinateString(getModule())),
                    "Formatted latitude/longitude"));

            // ===== Orbit basic =====
            AddSuffix(new[] { "MEANANOMALY", "MA" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MeanAnomaly(getModule())),
                    "Mean anomaly"));

            AddSuffix(new[] { "CURRENTORBITSUMMARY", "ORBSUM" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(CurrentOrbitSummary(getModule())),
                    "Orbit summary"));

            AddSuffix(new[] { "TARGETORBITSUMMARY", "TORBSUM" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitSummary(getModule())),
                    "Target orbit summary"));

            AddSuffix(new[] { "CURRENTORBITSUMMARYINC", "ORBSUMINC" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(CurrentOrbitSummaryWithInclination(getModule())),
                    "Orbit summary with inclination"));

            AddSuffix(new[] { "TARGETORBITSUMMARYINC", "TORBSUMINC" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitSummaryWithInclination(getModule())),
                    "Target orbit summary with inclination"));

            AddSuffix(new[] { "ORBITALENERGY", "ENERGY" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(OrbitalEnergy(getModule())),
                    "Specific orbital energy"));

            AddSuffix(new[] { "POTENTIALENERGY", "POT" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(PotentialEnergy(getModule())),
                    "Potential orbital energy"));

            AddSuffix(new[] { "KINETICENERGY", "KIN" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(KineticEnergy(getModule())),
                    "Kinetic energy"));

            AddSuffix(new[] { "TIMETOIMPACT", "IMPACT" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToImpact(getModule())),
                    "Time until impact"));

            AddSuffix(new[] { "SUICIDEBURNCOUNTDOWN", "SBC" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(SuicideBurnCountdown(getModule())),
                    "Time until suicide burn"));

            AddSuffix(new[] { "TIMETOSOITRANSITION", "SOIETA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToSOITransition(getModule())),
                    "Time to SOI transition"));

            AddSuffix(new[] { "SURFACEGRAVITY", "g" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(SurfaceGravity(getModule())),
                    "Surface gravity"));

            AddSuffix(new[] { "ESCAPEVELOCITY", "VESC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(EscapeVelocity(getModule())),
                    "Escape velocity"));

            AddSuffix(new[] { "CIRCULARORBITSPEED", "VCIRC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(CircularOrbitSpeed(getModule())),
                    "Circular orbit velocity"));

            // ===== RCS =====
            AddSuffix(new[] { "RCSTHRUST", "RCSF" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(RCSThrust(getModule())),
                    "RCS thrust"));

            AddSuffix(new[] { "RCSTRANSLATIONEFF", "RCSEFF" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(RCSTranslationEfficiency(getModule())),
                    "RCS translation efficiency"));

            AddSuffix(new[] { "RCSDELTAVVAC", "RCSDV" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(RCSDeltaVVacuum(getModule())),
                    "RCS ΔV in vacuum"));

            // ===== Angular velocity =====
            AddSuffix(new[] { "ANGULARVELOCITY", "ANGVEL" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(AngularVelocity(getModule())),
                    "Angular velocity"));

            // ===== Vessel basic =====
            AddSuffix(new[] { "VESSELNAME", "NAME" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(VesselName(getModule())),
                    "Vessel name"));

            AddSuffix(new[] { "VESSELTYPE", "TYPE" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(VesselType(getModule())),
                    "Vessel type"));

            AddSuffix(new[] { "VESSELMASS", "MASS" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(VesselMass(getModule())),
                    "Total vessel mass"));

            AddSuffix(new[] { "MAXVESSELMASS", "MASSMAX" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(MaximumVesselMass(getModule())),
                    "Maximum vessel mass"));

            AddSuffix(new[] { "DRYMASS", "DRYM" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(DryMass(getModule())),
                    "Dry mass"));

            AddSuffix(new[] { "LFO_MASS", "LFO" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(LiquidFuelAndOxidizerMass(getModule())),
                    "LFO mass"));

            AddSuffix(new[] { "MONOPROP_MASS", "MP" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(MonoPropellantMass(getModule())),
                    "MonoPropellant mass"));

            AddSuffix(new[] { "ELECTRICCHARGE", "EC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(TotalElectricCharge(getModule())),
                    "Electric charge"));

            AddSuffix(new[] { "PARTCOUNT", "PC" },
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(PartCount(getModule())),
                    "Part count"));

            AddSuffix(new[] { "MAXPARTCOUNT", "PCMAX" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(MaxPartCount(getModule())),
                    "Max part count"));

            AddSuffix(new[] { "PARTCOUNTANDMAX", "PCM" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(PartCountAndMaxPartCount(getModule())),
                    "Part count (with max)"));

            AddSuffix(new[] { "STRUTCOUNT", "STRUTS" },
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(StrutCount(getModule())),
                    "Strut count"));

            AddSuffix(new[] { "FUELLINESCOUNT", "LINES" },
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(FuelLinesCount(getModule())),
                    "Fuel lines count"));

            AddSuffix(new[] { "VESSELCOST", "COST" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(VesselCost(getModule())),
                    "Vessel cost"));

            AddSuffix(new[] { "CREWCOUNT", "CREW" },
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(CrewCount(getModule())),
                    "Crew on board"));

            AddSuffix(new[] { "CREWCAPACITY", "CAP" },
                new NoArgsSuffix<ScalarIntValue>(
                    () => new ScalarIntValue(CrewCapacity(getModule())),
                    "Crew capacity"));

            // ===== Target distance / relative motion =====
            AddSuffix(new[] { "TARGETDISTANCE", "TDIST" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetDistance(getModule())),
                    "Distance to target"));

            AddSuffix(new[] { "HEADINGTOTARGET", "THDG" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(HeadingToTarget(getModule())),
                    "Heading to target"));

            AddSuffix(new[] { "TARGETRELV", "TRELV" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetRelativeVelocity(getModule())),
                    "Relative velocity to target"));

            AddSuffix(new[] { "TARGETTTCLOSEAPP", "TCA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetTimeToClosestApproach(getModule())),
                    "ETA to closest approach"));

            AddSuffix(new[] { "TARGETCLOSEAPPDIST", "CADIST" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetClosestApproachDistance(getModule())),
                    "Closest approach distance"));

            AddSuffix(new[] { "TARGETCLOSEAPPRELV", "CAREL" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetClosestApproachRelativeVelocity(getModule())),
                    "Closest approach relative velocity"));

            // ===== Target SoI / capture =====
            AddSuffix(new[] { "PERIAPSISINTARGETSOI", "TPERI" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(PeriapsisInTargetSOI(getModule())),
                    "Periapsis in target SOI"));

            AddSuffix(new[] { "TARGETCAPTUREDV", "TCAPDV" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetCaptureDV(getModule())),
                    "Target capture ΔV"));

            // ===== Target orbit numbers =====
            AddSuffix(new[] { "TARGETAPOAPSIS", "TAPA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetApoapsis(getModule())),
                    "Target apoapsis"));

            AddSuffix(new[] { "TARGETPERIAPSIS", "TPEA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetPeriapsis(getModule())),
                    "Target periapsis"));

            AddSuffix(new[] { "TARGETINCLINATION", "TINCL" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetInclination(getModule())),
                    "Target inclination"));

            AddSuffix(new[] { "TARGETORBITPERIOD", "TORBITPER" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitPeriod(getModule())),
                    "Target orbit period"));

            AddSuffix(new[] { "TARGETORBITSPEED", "TORBVEL" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitSpeed(getModule())),
                    "Target orbital speed"));

            AddSuffix(new[] { "TARGETORBITTIMETOAP", "TTTOAP" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitTimeToAp(getModule())),
                    "Target time to apoapsis"));

            AddSuffix(new[] { "TARGETORBITTIMETOPE", "TTTOPE" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetOrbitTimeToPe(getModule())),
                    "Target time to periapsis"));

            AddSuffix(new[] { "TARGETLAN", "TLAN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetLAN(getModule())),
                    "Target longitude of ascending node"));

            AddSuffix(new[] { "TARGETLDN", "TLDN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetLDN(getModule())),
                    "Target longitude of descending node"));

            AddSuffix(new[] { "TARGETTIMETOASCENDINGNODE", "TTAN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetTimeToAscendingNode(getModule())),
                    "Target time to ascending node"));

            AddSuffix(new[] { "TARGETTIMETODESCENDINGNODE", "TTDN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetTimeToDescendingNode(getModule())),
                    "Target time to descending node"));

            AddSuffix(new[] { "TARGETAOP", "TAOP" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetAoP(getModule())),
                    "Target argument of periapsis"));

            AddSuffix(new[] { "TARGETECCENTRICITY", "TECC" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetEccentricity(getModule())),
                    "Target eccentricity"));

            AddSuffix(new[] { "TARGETSMA", "TSMA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetSMA(getModule())),
                    "Target semi-major axis"));

            AddSuffix(new[] { "TARGETMEANANOMALY", "TMA" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetMeanAnomaly(getModule())),
                    "Target mean anomaly"));

            AddSuffix(new[] { "TARGETTRUELONGITUDE", "TTLON" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetTrueLongitude(getModule())),
                    "Target true longitude"));

            // ===== Target / relative geometry =====
            AddSuffix(new[] { "SYNODICPERIOD", "SYNOD" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(SynodicPeriod(getModule())),
                    "Synodic period"));

            AddSuffix(new[] { "PHASEANGLE", "PHASE" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(PhaseAngle(getModule())),
                    "Phase angle"));

            AddSuffix(new[] { "TARGETPLANETPHASEANGLE", "TPHASE" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TargetPlanetPhaseAngle(getModule())),
                    "Target planet phase angle"));

            AddSuffix(new[] { "RELATIVEINCLINATIONTOTARGET", "RINCL" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(RelativeInclinationToTarget(getModule())),
                    "Relative inclination to target"));

            AddSuffix(new[] { "TIMETOASCENDINGNODEWITHTARGET", "TTANWT" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToAscendingNodeWithTarget(getModule())),
                    "Time to ascending node with target"));

            AddSuffix(new[] { "TIMETODESCENDINGNODEWITHTARGET", "TTDNWT" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToDescendingNodeWithTarget(getModule())),
                    "Time to descending node with target"));

            AddSuffix(new[] { "TIMETOEQUATORIALASCENDINGNODE", "TTEAN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToEquatorialAscendingNode(getModule())),
                    "Time to equatorial ascending node"));

            AddSuffix(new[] { "TIMETOEQUATORIALDESCENDINGNODE", "TTEDN" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TimeToEquatorialDescendingNode(getModule())),
                    "Time to equatorial descending node"));

            // ===== Biomes =====
            AddSuffix(new[] { "CURRENTRAWBIOME", "RAWBIOME" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(CurrentRawBiome(getModule())),
                    "Raw biome name"));

            AddSuffix(new[] { "CURRENTBIOME", "BIOME" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(CurrentBiome(getModule())),
                    "Biome name"));

            // ===== Stage dV / time =====
            AddSuffix(new[] { "STAGEDELTAVVAC", "SDV" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(StageDeltaVVacuum(getModule())),
                    "Stage ΔV vacuum"));

            AddSuffix(new[] { "STAGEDELTAVATM", "SDVATM" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(StageDeltaVAtmosphere(getModule())),
                    "Stage ΔV atmosphere"));

            AddSuffix(new[] { "STAGEDELTAVATMVAC", "SDVSTR" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(StageDeltaVAtmosphereAndVac(getModule())),
                    "Formatted ΔV atm/vac"));

            AddSuffix(new[] { "STAGETIMEFULL", "TBURNF" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(StageTimeLeftFullThrottle(getModule())),
                    "Stage burn time full throttle"));

            AddSuffix(new[] { "STAGETIMECURRENT", "TBURNC" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(StageTimeLeftCurrentThrottle(getModule())),
                    "Stage burn time current throttle"));

            AddSuffix(new[] { "STAGETIMEHOVER", "THOVER" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(StageTimeLeftHover(getModule())),
                    "Stage hover time"));

            AddSuffix(new[] { "TOTALDVVAC", "TDV" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(TotalDeltaVVaccum(getModule())),
                    "Total ΔV vacuum"));

            AddSuffix(new[] { "TOTALDVATM", "TDVATM" },
                new NoArgsSuffix<ScalarDoubleValue>(
                    () => new ScalarDoubleValue(TotalDeltaVAtmosphere(getModule())),
                    "Total ΔV atmosphere"));

            AddSuffix(new[] { "TOTALDVATMVAC", "TDVSTR" },
                new NoArgsSuffix<StringValue>(
                    () => new StringValue(TotalDeltaVAtmosphereAndVac(getModule())),
                    "Formatted total ΔV atm/vac"));
        }

        public override string context() => nameof(MechJebInfoItemsWrapper);

        private void BindInfoItems(Type coreType)
        {
            var wrapperType = GetType();

            foreach (var method in coreType.GetMethods(_methodFlags))
            {
                if (method.GetParameters().Length != 0)
                    continue;

                if (!method.HasAttributeNamed("ValueInfoItemAttribute"))
                    continue;
                
                var prop = wrapperType.GetProperty(method.Name, BindingFlags.Instance | BindingFlags.Public);
                if (prop == null)
                    continue;

                var propType = prop.PropertyType;

                if (propType == typeof(Func<object, string>))
                {
                    var del = GetOrCreateStringGetter(method);
                    prop.SetValue(this, del, null);
                }
                else if (propType == typeof(Func<object, double>))
                {
                    var del = GetOrCreateDoubleGetter(method);
                    prop.SetValue(this, del, null);
                }
                else if (propType == typeof(Func<object, int>))
                {
                    var del = GetOrCreateIntGetter(method);
                    prop.SetValue(this, del, null);
                }
                else if (propType == typeof(Func<object, float>))
                {
                    var del = GetOrCreateFloatGetter(method);
                    prop.SetValue(this, del, null);
                }
            }
        }

        private static Func<object, string> GetOrCreateStringGetter(MethodInfo method)
        {
            lock (_cacheLock)
            {
                if (_methodCache.TryGetValue(method, out var cached))
                    return (Func<object, string>)cached;

                var getter = method.BuildStringMethod();
                _methodCache[method] = getter;
                return getter;
            }
        }

        private static Func<object, double> GetOrCreateDoubleGetter(MethodInfo method)
        {
            lock (_cacheLock)
            {
                if (_methodCache.TryGetValue(method, out var cached))
                    return (Func<object, double>)cached;

                var getter = method.BuildDoubleMethod();
                _methodCache[method] = getter;
                return getter;
            }
        }

        private static Func<object, int> GetOrCreateIntGetter(MethodInfo method)
        {
            lock (_cacheLock)
            {
                if (_methodCache.TryGetValue(method, out var cached))
                    return (Func<object, int>)cached;

                var getter = method.BuildIntMethod();
                _methodCache[method] = getter;
                return getter;
            }
        }

        private static Func<object, float> GetOrCreateFloatGetter(MethodInfo method)
        {
            lock (_cacheLock)
            {
                if (_methodCache.TryGetValue(method, out var cached))
                    return (Func<object, float>)cached;

                var getter = method.BuildFloatMethod();
                _methodCache[method] = getter;
                return getter;
            }
        }
    }
}