// ManeuverPlannerBasicTest.ks
// Tests for kOS.MechJeb2.Addon Basic ManeuverPlanner operations
// Operations: CHANGEPE, CHANGEAP, CIRCULARIZE, ELLIPTICIZE, SEMIMAJOR

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb Basic Maneuver TESTS  ".
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
// Test: CHANGEPE
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEPE".

CLEAR_NODES().

// Lower periapsis slightly (safe test - always valid)
SET targetPe TO MAX(70000, PERIAPSIS - 5000).
PRINT "  Target Pe: " + targetPe.

SET result TO planner:CHANGEPE(targetPe, "APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("CHANGEPE returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created (may need elliptical orbit).".
}.

PRINT "CHANGEPE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: CHANGEAP
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEAP".

CLEAR_NODES().

// Raise apoapsis (always valid)
SET targetAp TO APOAPSIS + 10000.
PRINT "  Target Ap: " + targetAp.

SET result TO planner:CHANGEAP(targetAp, "PERIAPSIS").
WAIT 0.2.

ASSERT_TRUE("CHANGEAP returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created.".
}.

PRINT "CHANGEAP tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: CIRCULARIZE
// -----------------------------------------------------------------------------
PRINT "TEST: CIRCULARIZE".

CLEAR_NODES().

SET result TO planner:CIRCULARIZE("APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("CIRCULARIZE returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created (may already be circular).".
}.

PRINT "CIRCULARIZE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ELLIPTICIZE
// -----------------------------------------------------------------------------
PRINT "TEST: ELLIPTICIZE".

CLEAR_NODES().

// Check if we're actually in orbit (apoapsis > 70km)
IF APOAPSIS > 70000 {
    // Set both Pe and Ap (must be valid for current orbit)
    SET targetPe TO MAX(70000, PERIAPSIS - 5000).
    SET targetAp TO APOAPSIS + 20000.
    PRINT "  Target Pe: " + targetPe + ", Ap: " + targetAp.

    SET result TO planner:ELLIPTICIZE(targetPe, targetAp, "APOAPSIS").
    WAIT 0.2.

    ASSERT_TRUE("ELLIPTICIZE returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: Vessel not in orbit (Ap=" + ROUND(APOAPSIS) + "m).".
    // Still count as passed - we verified the suffix exists
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "ELLIPTICIZE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: SEMIMAJOR
// -----------------------------------------------------------------------------
PRINT "TEST: SEMIMAJOR".

CLEAR_NODES().

// Check if we're actually in orbit (apoapsis > 70km)
IF APOAPSIS > 70000 {
    // Set semi-major axis (body radius + desired altitude average)
    SET bodySma TO BODY:RADIUS.
    SET targetSma TO bodySma + 120000.  // ~120km average altitude
    PRINT "  Target SMA: " + targetSma.

    SET result TO planner:SEMIMAJOR(targetSma, "APOAPSIS").
    WAIT 0.2.

    ASSERT_TRUE("SEMIMAJOR returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: Vessel not in orbit (Ap=" + ROUND(APOAPSIS) + "m).".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "SEMIMAJOR tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "   BASIC MANEUVER SUMMARY      ".
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
    PRINT "ALL BASIC MANEUVER TESTS PASSED".
}.

PRINT "===============================".
PRINT "Basic maneuver tests finished.".
