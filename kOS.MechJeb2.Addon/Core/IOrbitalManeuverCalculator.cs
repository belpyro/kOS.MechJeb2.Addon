using kOS.Safe.Encapsulation;
using kOS.Suffixed;

namespace kOS.MechJeb2.Addon.Core
{
    /// <summary>
    /// Interface for orbital maneuver calculations originally implemented in <see cref="OrbitalManeuverCalculator"/>.
    /// </summary>
    public interface IOrbitalManeuverCalculator
    {
        /// <summary>
        /// Computes the deltaV of the burn needed to circularize an orbit at a given UT.
        /// </summary>
        Vector DeltaVToCircularize(Orbit o, double ut);

        /// <summary>
        /// Computes the deltaV of the burn needed to set a given PeR and ApR at a given UT.
        /// </summary>
        Vector DeltaVToEllipticize(Orbit o, double ut, double newPeR, double newApR);

        /// <summary>
        /// Computes the delta-V of the burn required to attain a given periapsis,
        /// starting from a given orbit and burning at a given UT.
        /// </summary>
        Vector DeltaVToChangePeriapsis(Orbit o, double ut, double newPeR);

        /// <summary>
        /// Computes the delta-V of the burn at a given UT required to change an orbit's apoapsis
        /// to a given value. Note that you can pass in a negative apoapsis if the desired
        /// final orbit is hyperbolic.
        /// </summary>
        Vector DeltaVToChangeApoapsis(Orbit o, double ut, double newApR);

        /// <summary>
        /// Computes the delta-V of the burn required to change an orbit's eccentricity
        /// to a given value at a given UT.
        /// </summary>
        Vector DeltaVToChangeEccentricity(Orbit o, double ut, double newEcc);

        /// <summary>
        /// Computes the delta-V of the burn required to obtain a given semi-major axis at a given UT.
        /// </summary>
        Vector DeltaVForSemiMajorAxis(Orbit o, double ut, double newSMA);

        /// <summary>
        /// Computes the heading for a ground launch at the specified latitude accounting for the body rotation.
        /// Both inputs are in degrees.
        ///
        /// Convention (at equator):
        /// inclination    0   =&gt; heading  90 (east)
        /// inclination   90   =&gt; heading   0 (north)
        /// inclination  -90   =&gt; heading 180 (south)
        /// inclination ±180   =&gt; heading 270 (west)
        ///
        /// Returned heading is in degrees and in the range 0 to 360.
        /// If the given latitude is too large, so that an orbit with a given inclination never attains the
        /// given latitude, then this function returns either 90 (if -90 &lt; inclination &lt; 90) or 270.
        /// </summary>
        ScalarDoubleValue HeadingForLaunchInclination(Orbit o, double inclinationDegrees, double desiredApoapsis);

        /// <summary>
        /// Computes the delta-V of the burn required to change an orbit's inclination to a given value
        /// at a given UT. If the latitude at that time is too high, so that the desired inclination
        /// cannot be attained, the burn returned will achieve as low an inclination as possible
        /// (namely, inclination = latitude).
        ///
        /// The input inclination is in degrees.
        /// Note that there are two orbits through each point with a given inclination. The convention used is:
        ///   - first, clamp newInclination to the range -180, 180
        ///   - if newInclination &gt; 0, do the cheaper burn to set that inclination
        ///   - if newInclination &lt; 0, do the more expensive burn to set that inclination
        /// </summary>
        Vector DeltaVToChangeInclination(Orbit o, double ut, double newInclination);

        /// <summary>
        /// Computes the delta-V and time of a burn to match planes with the target orbit.
        /// The output burnUT will be equal to the time of the first ascending node with respect
        /// to the target after the given UT.
        ///
        /// Throws an ArgumentException if <paramref name="o"/> is hyperbolic and doesn't
        /// have an ascending node relative to the target.
        /// </summary>
        Lexicon DeltaVAndTimeToMatchPlanesAscending(Orbit o, Orbit target, double ut);

        /// <summary>
        /// Computes the delta-V and time of a burn to match planes with the target orbit.
        /// The output burnUT will be equal to the time of the first descending node with respect
        /// to the target after the given UT.
        ///
        /// Throws an ArgumentException if <paramref name="o"/> is hyperbolic and doesn't
        /// have a descending node relative to the target.
        /// </summary>
        Lexicon DeltaVAndTimeToMatchPlanesDescending(Orbit o, Orbit target, double ut);

        /// <summary>
        /// Computes the time and dV of a Hohmann transfer injection burn such that at apoapsis
        /// the transfer orbit passes as close as possible to the target.
        ///
        /// The output UT1 will be the first transfer window found after the given UT.
        /// Assumes o and target are in approximately the same plane, and orbiting in the same direction.
        /// Also assumes that o is a perfectly circular orbit (though result should be OK for small eccentricity).
        /// </summary>
        (Vector dV1, double UT1, Vector dV2, double UT2) DeltaVAndTimeForHohmannTransfer(
            Orbit o,
            Orbit target,
            double ut,
            double lagTime = double.NaN,
            bool fixedTime = false,
            bool coplanar = true,
            bool rendezvous = true,
            bool capture = true);

        /// <summary>
        /// Computes the delta-V of a burn at a given time that will put an object with a given orbit
        /// on a course to intercept a target at a specific intercept time.
        ///
        /// offsetDistance: this is used by the Rendezvous Autopilot and is only going to be valid over very short distances.
        /// shortway: the shortway parameter to feed into the Lambert solver.
        /// </summary>
        (Vector v1, Vector v2) DeltaVToInterceptAtTime(
            Orbit o,
            double t0,
            Orbit target,
            double dt,
            double offsetDistance = 0,
            bool shortway = true);

        /// <summary>
        /// Does a line-search to find the burnUT for the cheapest course correction that will intercept exactly.
        /// </summary>
        Vector DeltaVAndTimeForCheapestCourseCorrection(Orbit o, double ut, Orbit target, out double burnUT);

        /// <summary>
        /// Entry point for the course-correction to a target orbit which is a celestial body.
        /// </summary>
        Vector DeltaVAndTimeForCheapestCourseCorrection(
            Orbit o,
            double ut,
            Orbit target,
            CelestialBody targetBody,
            double finalPeR,
            out double burnUT);

        /// <summary>
        /// Entry point for the course-correction to a target orbit which is not a celestial body.
        /// </summary>
        Vector DeltaVAndTimeForCheapestCourseCorrection(
            Orbit o,
            double ut,
            Orbit target,
            double caDistance,
            out double burnUT);

        /// <summary>
        /// Computes the time and delta-V of an ejection burn to a Hohmann transfer from one planet to another.
        ///
        /// It's assumed that the initial orbit around the first planet is circular, and that this orbit
        /// is in the same plane as the orbit of the first planet around the sun. It's also assumed that
        /// the target planet has a fairly low relative inclination with respect to the first planet.
        /// </summary>
        Vector DeltaVAndTimeForInterplanetaryTransferEjection(
            Orbit o,
            double ut,
            Orbit target,
            bool syncPhaseAngle,
            out double burnUT);

        /// <summary>
        /// Computes the delta-V and time for a moon-return ejection maneuver.
        /// </summary>
        (Vector dv, double dt) DeltaVAndTimeForMoonReturnEjection(Orbit o, double ut, double targetPrimaryRadius);

        /// <summary>
        /// Computes the delta-V of the burn at a given time required to zero out the difference
        /// in orbital velocities between a given orbit and a target.
        /// </summary>
        Vector DeltaVToMatchVelocities(Orbit o, double ut, Orbit target);

        /// <summary>
        /// Computes the delta-V of the burn at the given time required to enter an orbit with a period
        /// of (f - 1) / f of the starting orbit period.
        /// </summary>
        Vector DeltaVToResonantOrbit(Orbit o, double ut, double f);

        /// <summary>
        /// Compute the angular distance between two points on a unit sphere.
        /// </summary>
        ScalarDoubleValue Distance(double lat_a, double long_a, double lat_b, double long_b);

        /// <summary>
        /// Compute an angular heading from point a to point b on a unit sphere.
        ///
        /// Using Great-Circle Navigation formula for initial heading (see great-circle navigation).
        /// Original equation returns 0 for due south, increasing clockwise. We add 180 and clamp to
        /// 0–360 degrees to map to compass-type headings.
        /// </summary>
        ScalarDoubleValue Heading(double lat_a, double long_a, double lat_b, double long_b);

        /// <summary>
        /// Computes the deltaV of the burn needed to set a given LAN at a given UT.
        /// </summary>
        Vector DeltaVToShiftLAN(Orbit o, double ut, double newLAN);

        /// <summary>
        /// Computes the deltaV of the burn needed to shift the node longitude to a given value at a given UT.
        /// </summary>
        Vector DeltaVToShiftNodeLongitude(Orbit o, double ut, double newNodeLong);

        /// <summary>
        /// Runs the PatchedConicSolver to do initial value "shooting" given an initial orbit, a maneuver dV
        /// and UT to execute, to a target Celestial's SOI.
        ///
        /// initial   : initial parking orbit.
        /// target    : the body whose SOI we are shooting towards.
        /// dV        : the dV of the maneuver off of the parking orbit.
        /// burnUT    : the time of the maneuver off of the parking orbit.
        /// arrivalUT : this is really more of an upper clamp on the simulation so that if we miss and never hit
        ///             the body SOI it stops.
        /// intercept : this is the final computed intercept orbit; it should be in the SOI of the target body,
        ///             but if it never hits it then the e.g. heliocentric orbit is returned instead,
        ///             so the caller needs to check.
        ///
        /// FIXME: NREs when there's no next patch.
        /// FIXME: duplicates code with OrbitExtensions.CalculateNextOrbit().
        /// </summary>
        void PatchedConicInterceptBody(
            Orbit initial,
            CelestialBody target,
            Vector dV,
            double burnUT,
            double arrivalUT,
            out Orbit intercept);

        /// <summary>
        /// Takes an e.g. heliocentric orbit and a target planet celestial and finds the time of the SOI intercept.
        /// </summary>
        void SOI_intercept(Orbit transfer, CelestialBody target, double ut1, double ut2, out double ut);
    }
}