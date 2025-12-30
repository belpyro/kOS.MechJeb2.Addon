// TargetWrapperTest.ks
// Tests for kOS.MechJeb2.Addon TargetWrapper (ADDONS:MJ:TARGET)

CLEARSCREEN.
PRINT "===============================".
PRINT "   MechJeb TARGET TESTS        ".
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
// Getting target wrapper
// -----------------------------------------------------------------------------
PRINT "Getting TARGET wrapper...".
SET tgt TO ADDONS:MJ:TARGET.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Wrapper access
// -----------------------------------------------------------------------------
PRINT "TEST: Wrapper access".

ASSERT_TRUE("TARGET exists", DEFINED(tgt)).
ASSERT_TRUE("TARGET type check", tgt:ISTYPE("TargetWrapper")).

SET tgt2 TO ADDONS:MJ:TARGETCONTROLLER.
ASSERT_TRUE("TARGETCONTROLLER alias exists", DEFINED(tgt2)).

PRINT "Wrapper access tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Target exists flags
// -----------------------------------------------------------------------------
PRINT "TEST: Target exists flags".

SET normalExists TO tgt:NORMALTARGETEXISTS.
SET positionExists TO tgt:POSITIONTARGETEXISTS.

ASSERT_TRUE("NORMALTARGETEXISTS is boolean", normalExists = TRUE OR normalExists = FALSE).
ASSERT_TRUE("POSITIONTARGETEXISTS is boolean", positionExists = TRUE OR positionExists = FALSE).

PRINT "  NORMALTARGETEXISTS: " + normalExists.
PRINT "  POSITIONTARGETEXISTS: " + positionExists.

PRINT "Target exists flags tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Equatorial node times
// -----------------------------------------------------------------------------
PRINT "TEST: Equatorial node times".

SET timeToEqAN TO tgt:TIMETOEQAN.
SET timeToEqDN TO tgt:TIMETOEQDN.

ASSERT_TRUE("TIMETOEQAN is scalar", timeToEqAN:ISTYPE("Scalar")).
ASSERT_TRUE("TIMETOEQDN is scalar", timeToEqDN:ISTYPE("Scalar")).

PRINT "  TIMETOEQAN: " + ROUND(timeToEqAN) + " s".
PRINT "  TIMETOEQDN: " + ROUND(timeToEqDN) + " s".

PRINT "Equatorial node times tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Basic target info properties
// -----------------------------------------------------------------------------
PRINT "TEST: Basic target info".

SET targetName TO tgt:NAME.
SET targetDist TO tgt:DISTANCE.

ASSERT_TRUE("NAME is string", targetName:ISTYPE("String")).
ASSERT_TRUE("DISTANCE is scalar", targetDist:ISTYPE("Scalar")).

PRINT "  NAME: " + targetName.
PRINT "  DISTANCE: " + ROUND(targetDist/1000) + " km".

PRINT "Basic target info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Node existence flags
// -----------------------------------------------------------------------------
PRINT "TEST: Node existence".

SET anExists TO tgt:ANEXISTS.
SET dnExists TO tgt:DNEXISTS.

ASSERT_TRUE("ANEXISTS is boolean", anExists = TRUE OR anExists = FALSE).
ASSERT_TRUE("DNEXISTS is boolean", dnExists = TRUE OR dnExists = FALSE).

PRINT "  ANEXISTS: " + anExists.
PRINT "  DNEXISTS: " + dnExists.

PRINT "Node existence tests done.".
PRINT "-------------------------------".

// Note: Method tests (SETTARGET*, UNSET, etc.) are skipped in automated tests.
// Test manually: SET T TO ADDONS:MJ:TARGET. T:SETTARGETKSC.

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "     TARGET TEST SUMMARY       ".
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
    PRINT "ALL TARGET TESTS PASSED".
}.

PRINT "===============================".
PRINT "Target tests finished.".
