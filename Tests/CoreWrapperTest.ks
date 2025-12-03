// CoreWrapperTests.ks
// Tests for kOS.MechJeb2.Addon: MJ addon and MechJebCoreWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT "  MechJeb CoreWrapper TESTS    ".
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
// Getting addon and core
// -----------------------------------------------------------------------------
PRINT "Getting MJ addon and core...".
SET mj TO ADDONS:MJ.
SET mjcore TO mj:CORE.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ADDONS:MJ availability
// -----------------------------------------------------------------------------
PRINT "TEST: ADDONS:MJ availability".

ASSERT_TRUE("MJ:AVAILABLE is boolean", mj:AVAILABLE = TRUE OR mj:AVAILABLE = FALSE).

IF mj:AVAILABLE {
    PRINT "MechJeb reported as AVAILABLE.".
} ELSE {
    PRINT "WARNING: MechJeb reported as NOT AVAILABLE.".
}.

PRINT "ADDONS:MJ tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: CORE basic state (RUNNING)
// -----------------------------------------------------------------------------
PRINT "TEST: CORE basic properties".

ASSERT_TRUE("CORE:RUNNING is boolean", mjcore:RUNNING = TRUE OR mjcore:RUNNING = FALSE).

IF mjcore:RUNNING {
    PRINT "CORE is RUNNING.".
} ELSE {
    PRINT "WARNING: CORE is not RUNNING (addon may have refused to init).".
}.

PRINT "CORE basic tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "       CORE TEST SUMMARY       ".
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
    PRINT "ALL CORE TESTS PASSED ✅".
}.

PRINT "===============================".
PRINT "Core tests finished.".