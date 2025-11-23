using System;
using System.Reflection;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Utilities;

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

        protected override void BindObject()
        {
             BindInfoItems(CoreInstance.GetType());
        }

        protected override void InitializeSuffixes()
        {
            // ===== Maneuver / Node =====
            AddSufixInternal("NEXTMANEUVERNODEBURNTIME", NextManeuverNodeBurnTime, "Burn time for next maneuver node", "BURN");
            AddSufixInternal("TIMETOMANEUVERNODE", TimeToManeuverNode, "Time to next maneuver node", "NODEETA");
            AddSufixInternal("NEXTMANEUVERNODEDELTAV", NextManeuverNodeDeltaV, "Node delta-V", "NODEDV");

            // ===== TWR / thrust =====
            AddSufixInternal("SURFACETWR", (Delegate)SurfaceTWR, "Surface TWR", "STWR");
            AddSufixInternal("LOCALTWR", (Delegate)LocalTWR, "Local TWR", "LTWR");
            AddSufixInternal("THROTTLETWR", (Delegate)ThrottleTWR, "TWR at current throttle", "TTWR");
            AddSufixInternal("CURRENTACC", (Delegate)CurrentAcceleration, "Current acceleration", "ACC");
            AddSufixInternal("CURRENTTHRUST", (Delegate)CurrentThrust, "Current thrust (kN)", "THRUST");
            AddSufixInternal("MAXTHRUST", (Delegate)MaxThrust, "Max possible thrust", "MAXTH");
            AddSufixInternal("MINTHRUST", (Delegate)MinThrust, "Minimum engine thrust", "MINTH");
            AddSufixInternal("MAXACC", (Delegate)MaxAcceleration, "Max acceleration", "MAXACC");
            AddSufixInternal("MINACC", (Delegate)MinAcceleration, "Minimum acceleration", "MINACC");
            AddSufixInternal("ACCELERATION", (Delegate)Acceleration, "Net acceleration", "A");

            // ===== Atmosphere =====
            AddSufixInternal("ATMPRESSUREKPA", (Delegate)AtmosphericPressurekPA, "Atmospheric pressure (kPa)", "P_KPA");
            AddSufixInternal("ATMPRESSURE", (Delegate)AtmosphericPressure, "Atmospheric pressure (atm?)", "P");
            AddSufixInternal("ATMDESITYDRAG", (Delegate)AtmosphericDrag, "Drag force estimation", "DRAG");
            AddSufixInternal("DRAGCOEF", (Delegate)DragCoefficient, "Aerodynamic drag coefficient", "CD");

            // ===== Coordinates =====
            AddSufixInternal("COORDINATESTRING", GetCoordinateString, "Formatted latitude/longitude", "COORD");

            // ===== Orbit basics =====
            AddSufixInternal("MEANANOMALY", (Delegate)MeanAnomaly, "Mean anomaly", "MA");
            AddSufixInternal("CURRENTORBITSUMMARY", CurrentOrbitSummary, "Orbit summary", "ORBSUM");
            AddSufixInternal("TARGETORBITSUMMARY", TargetOrbitSummary, "Target orbit summary", "TORBSUM");
            AddSufixInternal("CURRENTORBITSUMMARYINC", CurrentOrbitSummaryWithInclination, "Orbit summary incl.", "ORBSUMINC");
            AddSufixInternal("TARGETORBITSUMMARYINC", TargetOrbitSummaryWithInclination, "Target orbit summary incl.",
                "TORBSUMINC");
            AddSufixInternal("ORBITALENERGY", (Delegate)OrbitalEnergy, "Specific orbital energy", "ENERGY");
            AddSufixInternal("POTENTIALENERGY", (Delegate)PotentialEnergy, "Potential orbital energy", "POT");
            AddSufixInternal("KINETICENERGY", (Delegate)KineticEnergy, "Kinetic energy", "KIN");
            AddSufixInternal("TIMETOIMPACT", TimeToImpact, "Time until impact", "IMPACT");
            AddSufixInternal("SUICIDEBURNCOUNTDOWN", SuicideBurnCountdown, "Time until suicide burn", "SBC");
            AddSufixInternal("TIMETOSOITRANSITION", TimeToSOITransition, "Time to SOI transition", "SOIETA");
            AddSufixInternal("SURFACEGRAVITY", (Delegate)SurfaceGravity, "Surface gravity", "g");
            AddSufixInternal("ESCAPEVELOCITY", (Delegate)EscapeVelocity, "Escape velocity", "VESC");
            AddSufixInternal("CIRCULARORBITSPEED", (Delegate)CircularOrbitSpeed, "Circular orbit velocity", "VCIRC");

            // ===== RCS =====
            AddSufixInternal("RCSTHRUST", (Delegate)RCSThrust, "RCS thrust", "RCSF");
            AddSufixInternal("RCSTRANSLATIONEFF", RCSTranslationEfficiency, "RCS translation eff", "RCSEFF");
            AddSufixInternal("RCSDELTAVVAC", (Delegate)RCSDeltaVVacuum, "RCS ΔV vacuum", "RCSDV");

            // ===== Angular =====
            AddSufixInternal("ANGULARVELOCITY", AngularVelocity, "Angular velocity", "ANGVEL");

            // ===== Vessel basic =====
            AddSufixInternal("VESSELNAME", VesselName, "Vessel name", "NAME");
            AddSufixInternal("VESSELTYPE", VesselType, "Vessel type", "TYPE");
            AddSufixInternal("VESSELMASS", (Delegate)VesselMass, "Total vessel mass", "MASS");
            AddSufixInternal("MAXVESSELMASS", MaximumVesselMass, "Maximum vessel mass", "MASSMAX");
            AddSufixInternal("DRYMASS", (Delegate)DryMass, "Dry mass", "DRYM");
            AddSufixInternal("LFO_MASS", (Delegate)LiquidFuelAndOxidizerMass, "LFO mass", "LFO");
            AddSufixInternal("MONOPROP_MASS", (Delegate)MonoPropellantMass, "MonoPropellant mass", "MP");
            AddSufixInternal("ELECTRICCHARGE", (Delegate)TotalElectricCharge, "Electric charge", "EC");
            AddSufixInternal("PARTCOUNT", PartCount, "Part count", "PC");
            AddSufixInternal("MAXPARTCOUNT", MaxPartCount, "Max part count", "PCMAX");
            AddSufixInternal("PARTCOUNTANDMAX", PartCountAndMaxPartCount, "Part count (with max)", "PCM");
            AddSufixInternal("STRUTCOUNT", StrutCount, "Strut count", "STRUTS");
            AddSufixInternal("FUELLINESCOUNT", FuelLinesCount, "Fuel lines count", "LINES");
            AddSufixInternal("VESSELCOST", (Delegate)VesselCost, "Vessel cost", "COST");
            AddSufixInternal("CREWCOUNT", CrewCount, "Crew on board", "CREW");
            AddSufixInternal("CREWCAPACITY", CrewCapacity, "Crew capacity", "CAP");

            // ===== Target relative motion =====
            AddSufixInternal("TARGETDISTANCE", TargetDistance, "Distance to target", "TDIST");
            AddSufixInternal("HEADINGTOTARGET", HeadingToTarget, "Heading to target", "THDG");
            AddSufixInternal("TARGETRELV", TargetRelativeVelocity, "Relative velocity", "TRELV");
            AddSufixInternal("TARGETTTCLOSEAPP", TargetTimeToClosestApproach, "ETA to closest approach", "TCA");
            AddSufixInternal("TARGETCLOSEAPPDIST", TargetClosestApproachDistance, "Closest approach dist", "CADIST");
            AddSufixInternal("TARGETCLOSEAPPRELV", TargetClosestApproachRelativeVelocity, "Closest rel vel", "CAREL");

            // ===== Biomes =====
            AddSufixInternal("CURRENTRAWBIOME", CurrentRawBiome, "Raw biome name", "RAWBIOME");
            AddSufixInternal("CURRENTBIOME", CurrentBiome, "Biome name", "BIOME");

            // ===== Stage data =====
            AddSufixInternal("STAGEDELTAVVAC", (Delegate)StageDeltaVVacuum, "Stage ΔV vacuum", "SDV");
            AddSufixInternal("STAGEDELTAVATM", (Delegate)StageDeltaVAtmosphere, "Stage ΔV atmosphere", "SDVATM");
            AddSufixInternal("STAGEDELTAVATMVAC", StageDeltaVAtmosphereAndVac, "Formatted ΔV atm/vac", "SDVSTR");
            AddSufixInternal("STAGETIMEFULL", StageTimeLeftFullThrottle, "Stage burn time full throttle", "TBURNF");
            AddSufixInternal("STAGETIMECURRENT", StageTimeLeftCurrentThrottle, "Stage burn time current throttle", "TBURNC");
            AddSufixInternal("STAGETIMEHOVER", StageTimeLeftHover, "Stage hover time", "THOVER");
            AddSufixInternal("TOTALDVVAC", (Delegate)TotalDeltaVVaccum, "Total ΔV vacuum", "TDV");
            AddSufixInternal("TOTALDVATM", (Delegate)TotalDeltaVAtmosphere, "Total ΔV atmosphere", "TDVATM");
            AddSufixInternal("TOTALDVATMVAC", TotalDeltaVAtmosphereAndVac, "Formatted total ΔV atm/vac", "TDVSTR");
        }

        private void BindInfoItems(Type coreType)
        {
            var wrapperType = GetType();

            foreach (var method in coreType.GetMethods(_methodFlags))
            {
                // только методы без параметров с атрибутом ValueInfoItem
                if (method.GetParameters().Length != 0)
                    continue;

                if (!method.HasAttributeNamed("ValueInfoItemAttribute"))
                    continue;

                // ищем одноимённое свойство в враппере
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