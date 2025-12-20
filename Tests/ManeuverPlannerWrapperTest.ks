// ManeuverPlannerWrapperTest.ks
// Tests for kOS.MechJeb2.Addon ManeuverPlannerWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb PlannerWrapper TESTS  ".
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

// -----------------------------------------------------------------------------
// Getting planner wrapper
// -----------------------------------------------------------------------------
PRINT "Getting ManeuverPlanner wrapper...".
SET planner TO ADDONS:MJ:PLANNER.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: PLANNER access
// -----------------------------------------------------------------------------
PRINT "TEST: PLANNER access".

ASSERT_TRUE("planner exists", DEFINED(planner)).
ASSERT_TRUE("planner type check", planner:ISTYPE("ManeuverPlannerWrapper")).

PRINT "PLANNER access tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: MANEUVERPLANNER alias
// -----------------------------------------------------------------------------
PRINT "TEST: MANEUVERPLANNER alias".

SET planner2 TO ADDONS:MJ:MANEUVERPLANNER.
ASSERT_TRUE("alias exists", DEFINED(planner2)).
ASSERT_TRUE("alias type check", planner2:ISTYPE("ManeuverPlannerWrapper")).

PRINT "MANEUVERPLANNER alias tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: OPERATIONS list
// -----------------------------------------------------------------------------
PRINT "TEST: OPERATIONS list".

SET ops TO planner:OPERATIONS.
ASSERT_TRUE("OPERATIONS returns list", ops:LENGTH > 0).

// Verify some known operations exist
SET foundPeriapsis TO FALSE.
SET foundApoapsis TO FALSE.
SET foundCircularize TO FALSE.
FOR op IN ops {
    LOCAL opLower IS op:TOLOWER().
    IF opLower:CONTAINS("periapsis") { SET foundPeriapsis TO TRUE. }
    IF opLower:CONTAINS("apoapsis") { SET foundApoapsis TO TRUE. }
    IF opLower:CONTAINS("circular") { SET foundCircularize TO TRUE. }
}

ASSERT_TRUE("contains periapsis operation", foundPeriapsis).
ASSERT_TRUE("contains apoapsis operation", foundApoapsis).
ASSERT_TRUE("contains circularize operation", foundCircularize).

PRINT "OPERATIONS list tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Node creation with CHANGEPE (requires orbit)
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEPE node creation".

// Clear any existing nodes first
UNTIL NOT HASNODE {
    REMOVE NEXTNODE.
    WAIT 0.1.
}
WAIT 0.1.

// Create a node (lower Pe slightly from current)
// Use a safe target that won't cause issues
SET targetPe TO MAX(70000, PERIAPSIS - 5000).
PRINT "  Creating node to change Pe to " + targetPe.

SET result TO planner:CHANGEPE(targetPe, "APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("CHANGEPE returns boolean", result = TRUE OR result = FALSE).

IF result {
    ASSERT_TRUE("node created", HASNODE).
    PRINT "  Node created successfully.".

    // Cleanup - remove the test node
    REMOVE NEXTNODE.
    WAIT 0.1.
    ASSERT_TRUE("node removed", NOT HASNODE).
} ELSE {
    PRINT "  Note: CHANGEPE returned false (may need different orbit state).".
}

PRINT "CHANGEPE node creation tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Node creation with CIRCULARIZE
// -----------------------------------------------------------------------------
PRINT "TEST: CIRCULARIZE node creation".

// Clear any existing nodes
UNTIL NOT HASNODE {
    REMOVE NEXTNODE.
    WAIT 0.1.
}
WAIT 0.1.

SET result TO planner:CIRCULARIZE("APOAPSIS").
WAIT 0.2.

ASSERT_TRUE("CIRCULARIZE returns boolean", result = TRUE OR result = FALSE).

IF result {
    ASSERT_TRUE("circularize node created", HASNODE).
    PRINT "  Node created successfully.".

    // Cleanup
    REMOVE NEXTNODE.
    WAIT 0.1.
} ELSE {
    PRINT "  Note: CIRCULARIZE returned false (may need different orbit state).".
}

PRINT "CIRCULARIZE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "     PLANNER TEST SUMMARY      ".
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
    PRINT "ALL PLANNER TESTS PASSED".
}.

PRINT "===============================".
PRINT "ManeuverPlanner tests finished.".
