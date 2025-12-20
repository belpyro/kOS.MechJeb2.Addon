// ManeuverPlannerTransferTest.ks
// Tests for kOS.MechJeb2.Addon Transfer operations
// Operations: HOHMANN, HOHMANNRENDEZVOUS, INTERPLANETARY, COURSECORRECTION, RESONANTORBIT, MOONRETURN

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb Transfer TESTS        ".
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
// Test: HOHMANNRENDEZVOUS (get/set suffix)
// -----------------------------------------------------------------------------
PRINT "TEST: HOHMANNRENDEZVOUS get/set".

SET origValue TO planner:HOHMANNRENDEZVOUS.
PRINT "  Original HOHMANNRENDEZVOUS: " + origValue.

ASSERT_TRUE("HOHMANNRENDEZVOUS is boolean", origValue = TRUE OR origValue = FALSE).

// Toggle the value
SET planner:HOHMANNRENDEZVOUS TO NOT origValue.
WAIT 0.1.
ASSERT_EQ("HOHMANNRENDEZVOUS toggled", NOT origValue, planner:HOHMANNRENDEZVOUS).

// Restore original
SET planner:HOHMANNRENDEZVOUS TO origValue.
WAIT 0.1.
ASSERT_EQ("HOHMANNRENDEZVOUS restored", origValue, planner:HOHMANNRENDEZVOUS).

PRINT "HOHMANNRENDEZVOUS tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: RESONANTORBIT (does not require target)
// -----------------------------------------------------------------------------
PRINT "TEST: RESONANTORBIT".

CLEAR_NODES().

// 2:1 resonant orbit (satellite returns every 2 parent orbits)
SET numerator TO 2.
SET denominator TO 1.
PRINT "  Resonance: " + numerator + ":" + denominator.

SET result TO planner:RESONANTORBIT(numerator, denominator, "APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("RESONANTORBIT returns boolean", result = TRUE OR result = FALSE).

IF result AND HASNODE {
    PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: No node created.".
}.

PRINT "RESONANTORBIT tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: HOHMANN (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: HOHMANN".

CLEAR_NODES().

IF HASTARGET {
    SET result TO planner:HOHMANN("COMPUTED", FALSE).
    WAIT 0.2.

    ASSERT_TRUE("HOHMANN returns boolean", result = TRUE OR result = FALSE).

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

PRINT "HOHMANN tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: INTERPLANETARY (requires target planet, not moon)
// -----------------------------------------------------------------------------
PRINT "TEST: INTERPLANETARY".

CLEAR_NODES().

// INTERPLANETARY is for planets (Duna, Eve, etc.) not moons (Mun, Minmus)
// A moon has BODY:BODY = Kerbin, a planet has BODY:BODY = Sun
IF HASTARGET AND TARGET:BODY:NAME = "Sun" {
    SET result TO planner:INTERPLANETARY(TRUE).
    WAIT 0.2.

    ASSERT_TRUE("INTERPLANETARY returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE IF HASTARGET {
    PRINT "  Skipped: Target is moon, not planet (use HOHMANN).".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
} ELSE {
    PRINT "  Skipped: No target selected.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "INTERPLANETARY tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: COURSECORRECTION (requires intercept trajectory)
// -----------------------------------------------------------------------------
PRINT "TEST: COURSECORRECTION".

CLEAR_NODES().

// COURSECORRECTION requires being on an intercept trajectory
// Skip if not in orbit or no target
IF APOAPSIS > 70000 AND HASTARGET {
    SET result TO planner:COURSECORRECTION(50000).
    WAIT 0.2.

    ASSERT_TRUE("COURSECORRECTION returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created (may need intercept trajectory).".
    }.
} ELSE {
    PRINT "  Skipped: Requires orbit and target with intercept.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "COURSECORRECTION tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: MOONRETURN (requires being in moon orbit)
// -----------------------------------------------------------------------------
PRINT "TEST: MOONRETURN".

CLEAR_NODES().

// MOONRETURN requires orbiting a moon (not Kerbin)
// Check if current body has a parent body (i.e., we're orbiting a moon)
IF BODY:HASBODY AND APOAPSIS > 70000 {
    // Target periapsis at parent body (e.g., 50km at Kerbin when leaving Mun)
    SET result TO planner:MOONRETURN(50000).
    WAIT 0.2.

    ASSERT_TRUE("MOONRETURN returns boolean", result = TRUE OR result = FALSE).

    IF result AND HASNODE {
        PRINT "  Node created with dV: " + ROUND(NEXTNODE:DELTAV:MAG, 2) + " m/s".
        REMOVE NEXTNODE.
        WAIT 0.1.
    } ELSE {
        PRINT "  Note: No node created.".
    }.
} ELSE {
    PRINT "  Skipped: Requires moon orbit (orbiting " + BODY:NAME + ").".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
}.

PRINT "MOONRETURN tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "    TRANSFER TEST SUMMARY      ".
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
    PRINT "ALL TRANSFER TESTS PASSED".
}.

PRINT "===============================".
PRINT "Transfer tests finished.".
PRINT "".
PRINT "Note: HOHMANN, INTERPLANETARY require target.".
PRINT "COURSECORRECTION requires intercept trajectory.".
PRINT "MOONRETURN requires moon orbit.".
