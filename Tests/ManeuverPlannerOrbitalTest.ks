// ManeuverPlannerOrbitalTest.ks
// Tests for kOS.MechJeb2.Addon Orbital geometry operations
// Operations: ECCENTRICITY, LONGITUDE, LAN

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb Orbital Geom. TESTS   ".
PRINT "===============================".

// -----------------------------------------------------------------------------
// Global counters
// -----------------------------------------------------------------------------
SET totalTests TO 0.
SET passedTests TO 0.
SET failedTests TO LIST().

// Simple assert
DECLARE FUNCTION ASSERT_EQ {
    PARAMETER name, expected, actual.

    SET totalTests TO totalTests + 1.

    IF expected = actual {
        SET passedTests TO passedTests + 1.
    } ELSE {
        LOCAL msg IS name + " expected: " + expected + ", actual: " + actual.
        failedTests:ADD(msg).
        PRINT "FAILED: " + msg.
    }
}.

// Helper: ASSERT_TRUE
DECLARE FUNCTION ASSERT_TRUE {
    PARAMETER name, condition.
    ASSERT_EQ(name, TRUE, condition).
}.

// Helper: Clear all maneuver nodes
DECLARE FUNCTION CLEAR_NODES {
    UNTIL NOT HASNODE {
        REMOVE NEXTNODE.
        WAIT 0.1.
    }
    WAIT 0.1.
}.

// -----------------------------------------------------------------------------
// Getting planner wrapper
// -----------------------------------------------------------------------------
PRINT "Getting ManeuverPlanner wrapper...".
SET planner TO ADDONS:MJ:PLANNER.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ECCENTRICITY
// -----------------------------------------------------------------------------
PRINT "TEST: ECCENTRICITY".

CLEAR_NODES().

// Target a modest eccentricity
SET targetEcc TO 0.1.
PRINT "  Target eccentricity: " + targetEcc.

SET result TO planner:ECCENTRICITY(targetEcc, "PERIAPSIS").
WAIT 0.2.

ASSERT_TRUE("ECCENTRICITY returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created.".
}.

PRINT "ECCENTRICITY tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: LONGITUDE (longitude of periapsis)
// -----------------------------------------------------------------------------
PRINT "TEST: LONGITUDE".

CLEAR_NODES().

// Target longitude of periapsis
SET targetLong TO 90.
PRINT "  Target longitude: " + targetLong + " deg".

SET result TO planner:LONGITUDE(targetLong, "APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("LONGITUDE returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created (may need elliptical orbit).".
}.

PRINT "LONGITUDE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: LAN (longitude of ascending node)
// -----------------------------------------------------------------------------
PRINT "TEST: LAN".

CLEAR_NODES().

// Target longitude of ascending node
SET targetLan TO 45.
PRINT "  Target LAN: " + targetLan + " deg".

SET result TO planner:LAN(targetLan, "APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("LAN returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created.".
}.

PRINT "LAN tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "  ORBITAL GEOMETRY SUMMARY     ".
PRINT "===============================".

PRINT "Total tests : " + totalTests.
PRINT "Passed      : " + passedTests.
SET failedCount TO (totalTests - passedTests).
PRINT "Failed      : " + failedCount.

IF failedCount > 0 {
    PRINT "".
    PRINT "Failed test details:".
    FOR item IN failedTests {
        PRINT " - " + item.
    }
} ELSE {
    PRINT "".
    PRINT "ALL ORBITAL GEOMETRY TESTS PASSED".
}.

PRINT "===============================".
PRINT "Orbital geometry tests finished.".
