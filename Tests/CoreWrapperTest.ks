// CoreWrapperTests.ks
// Tests for kOS.MechJeb2.Addon top-level addon and MechJebCoreWrapper

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

// -----------------------------------------------------------------------------
// Getting addon and core
// -----------------------------------------------------------------------------
PRINT "Getting MJ addon and core...".
SET mj TO ADDONS:MJ.
SET core TO mj:CORE.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ADDONS:MJ basic availability
// -----------------------------------------------------------------------------
PRINT "TEST: ADDONS:MJ availability".

ASSERT_EQ("MJ is not NONE", TRUE, mj <> NONE).
ASSERT_EQ("MJ:AVAILABLE is bool-like", TRUE, (mj:AVAILABLE = TRUE) OR (mj:AVAILABLE = FALSE)).

IF mj:AVAILABLE {
    PRINT "MechJeb reported as AVAILABLE.".
} ELSE {
    PRINT "WARNING: MechJeb reported as NOT AVAILABLE.".
}.

PRINT "ADDONS:MJ tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: CORE basic state
// -----------------------------------------------------------------------------
PRINT "TEST: CORE basic properties".

ASSERT_EQ("CORE is not NONE", TRUE, core <> NONE).
ASSERT_EQ("CORE:RUNNING is bool-like", TRUE, (core:RUNNING = TRUE) OR (core:RUNNING = FALSE)).

IF core:RUNNING {
    PRINT "CORE is RUNNING.".
} ELSE {
    PRINT "WARNING: CORE is not RUNNING.".
}.

PRINT "CORE basic tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: CORE sub-wrappers existence
// -----------------------------------------------------------------------------
PRINT "TEST: CORE sub-wrappers".

SET v TO core:VESSEL.
SET info TO core:INFO.
SET asc TO core:ASCENT.

ASSERT_EQ("CORE:VESSEL not NONE", TRUE, v <> NONE).
ASSERT_EQ("CORE:INFO not NONE", TRUE, info <> NONE).
ASSERT_EQ("CORE:ASCENT not NONE", TRUE, asc <> NONE).

PRINT "CORE sub-wrappers tests done.".
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