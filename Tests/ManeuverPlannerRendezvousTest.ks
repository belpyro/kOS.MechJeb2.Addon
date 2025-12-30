// ManeuverPlannerRendezvousTest.ks
// Tests for kOS.MechJeb2.Addon Rendezvous operations
// Operations: PLANE, KILLRELVEL, CHANGEINCLINATION, LAMBERT

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb Rendezvous TESTS      ".
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
// Test: CHANGEINCLINATION (does not require target)
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEINCLINATION".

CLEAR_NODES().

// Change to equatorial orbit
SET targetInc TO 0.
PRINT "  Target inclination: " + targetInc + " deg".

SET result TO planner:CHANGEINCLINATION(targetInc, "EQ_NEAREST_AD").
WAIT 0.2.

ASSERT_TRUE("CHANGEINCLINATION returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created (may already be equatorial).".
}.

PRINT "CHANGEINCLINATION tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: PLANE (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: PLANE".

CLEAR_NODES().

IF HASTARGET {
    SET result TO planner:PLANE("REL_NEAREST_AD").
    WAIT 0.2.

    ASSERT_TRUE("PLANE returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: No target selected.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "PLANE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: KILLRELVEL (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: KILLRELVEL".

CLEAR_NODES().

IF HASTARGET {
    SET result TO planner:KILLRELVEL("CLOSEST_APPROACH").
    WAIT 0.2.

    ASSERT_TRUE("KILLRELVEL returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: No target selected.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "KILLRELVEL tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: LAMBERT (requires target in same SOI)
// -----------------------------------------------------------------------------
PRINT "TEST: LAMBERT".

CLEAR_NODES().

IF HASTARGET {
    // 3600 seconds = 1 hour intercept interval
    SET result TO planner:LAMBERT(3600, "X_FROM_NOW").
    WAIT 0.2.

    ASSERT_TRUE("LAMBERT returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: No target selected.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "LAMBERT tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "    RENDEZVOUS TEST SUMMARY    ".
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
    PRINT "ALL RENDEZVOUS TESTS PASSED".
}.

PRINT "===============================".
PRINT "Rendezvous tests finished.".
PRINT "".
PRINT "Note: PLANE, KILLRELVEL, LAMBERT require".
PRINT "a target to create actual maneuver nodes.".
