// NodeExecutorWrapperTest.ks
// Tests for kOS.MechJeb2.Addon NodeExecutorWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb NodeExecutor TESTS    ".
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
// Getting node executor wrapper
// -----------------------------------------------------------------------------
PRINT "Getting NodeExecutor wrapper...".
SET mjnode TO ADDONS:MJ:NODE.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: NODE access
// -----------------------------------------------------------------------------
PRINT "TEST: NODE access".

ASSERT_TRUE("node wrapper exists", DEFINED(mjnode)).
ASSERT_TRUE("node type check", mjnode:ISTYPE("NodeExecutorWrapper")).

PRINT "NODE access tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: NODEEXECUTOR alias
// -----------------------------------------------------------------------------
PRINT "TEST: NODEEXECUTOR alias".

SET node2 TO ADDONS:MJ:NODEEXECUTOR.
ASSERT_TRUE("alias exists", DEFINED(node2)).
ASSERT_TRUE("alias type check", node2:ISTYPE("NodeExecutorWrapper")).

PRINT "NODEEXECUTOR alias tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ENABLED read
// -----------------------------------------------------------------------------
PRINT "TEST: ENABLED read".

SET enabled TO mjnode:ENABLED.
ASSERT_TRUE("ENABLED is boolean", enabled = TRUE OR enabled = FALSE).

// Should be FALSE when no execution is happening
PRINT "  ENABLED value: " + enabled.

PRINT "ENABLED read tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTOWARP get/set
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOWARP toggle".

SET origWarp TO mjnode:AUTOWARP.
PRINT "  Original AUTOWARP: " + origWarp.

// Toggle AUTOWARP
SET mjnode:AUTOWARP TO NOT origWarp.
WAIT 0.1.
ASSERT_EQ("AUTOWARP toggled", NOT origWarp, mjnode:AUTOWARP).

// Restore original
SET mjnode:AUTOWARP TO origWarp.
WAIT 0.1.
ASSERT_EQ("AUTOWARP restored", origWarp, mjnode:AUTOWARP).

PRINT "AUTOWARP toggle tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: WARP alias
// -----------------------------------------------------------------------------
PRINT "TEST: WARP alias".

SET origWarp TO mjnode:WARP.

// Set via WARP alias
SET mjnode:WARP TO NOT origWarp.
WAIT 0.1.
ASSERT_EQ("WARP alias set", NOT origWarp, mjnode:WARP).
ASSERT_EQ("AUTOWARP matches WARP", mjnode:WARP, mjnode:AUTOWARP).

// Restore
SET mjnode:WARP TO origWarp.
WAIT 0.1.
ASSERT_EQ("WARP alias restored", origWarp, mjnode:WARP).

PRINT "WARP alias tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "   NODE EXECUTOR TEST SUMMARY  ".
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
    PRINT "ALL NODE EXECUTOR TESTS PASSED".
}.

PRINT "===============================".
PRINT "NodeExecutor tests finished.".
