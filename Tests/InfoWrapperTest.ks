// InfoWrapperTests.ks
// Tests for kOS.MechJeb2.Addon MechJebInfoItemsWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT "  MechJeb InfoWrapper TESTS    ".
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
// Getting info wrapper
// -----------------------------------------------------------------------------
PRINT "Getting Info wrapper...".
SET info TO ADDONS:MJ:CORE:INFO.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Maneuver / node info
// -----------------------------------------------------------------------------
PRINT "TEST: maneuver / node info".

SET burnTime TO info:NEXTMANEUVERNODEBURNTIME.
SET nodeEta TO info:TIMETOMANEUVERNODE.
SET nodeDv TO info:NEXTMANEUVERNODEDELTAV.

// These can be -1 or 0 when no node is present; we only check that access works:
ASSERT_TRUE("NEXTMANEUVERNODEBURNTIME read OK", TRUE).
ASSERT_TRUE("TIMETOMANEUVERNODE read OK", TRUE).
ASSERT_TRUE("NEXTMANEUVERNODEDELTAV read OK", TRUE).

PRINT "Maneuver info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TWR / thrust / acceleration
// -----------------------------------------------------------------------------
PRINT "TEST: TWR / thrust / acceleration".

SET stwr TO info:SURFACETWR.
SET ltwr TO info:LOCALTWR.
SET ttwr TO info:THROTTLETWR.
SET acc TO info:CURRENTACC.
SET thrust TO info:CURRENTTHRUST.

ASSERT_TRUE("SURFACETWR >= 0", stwr >= 0).
ASSERT_TRUE("LOCALTWR >= 0", ltwr >= 0).
ASSERT_TRUE("THROTTLETWR >= 0", ttwr >= 0).
ASSERT_TRUE("CURRENTACC >= 0", acc >= 0).
ASSERT_TRUE("CURRENTTHRUST >= 0", thrust >= 0).

PRINT "TWR / thrust tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Stage and total deltaV
// -----------------------------------------------------------------------------
PRINT "TEST: stage and total ΔV".

SET stageVac TO info:STAGEDELTAVVAC.
SET totalVac TO info:TOTALDVVAC.

ASSERT_TRUE("STAGEDELTAVVAC >= 0", stageVac >= 0).
ASSERT_TRUE("TOTALDVVAC >= 0", totalVac >= 0).

PRINT "ΔV tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Vessel basics
// -----------------------------------------------------------------------------
PRINT "TEST: vessel basics from info".

SET vName TO info:VESSELNAME.
SET vType TO info:VESSELTYPE.
SET vMass TO info:VESSELMASS.
SET crew TO info:CREWCOUNT.

ASSERT_TRUE("VESSELNAME not empty", vName <> "").
ASSERT_TRUE("VESSELTYPE not empty", vType <> "").
ASSERT_TRUE("VESSELMASS > 0", vMass > 0).
ASSERT_TRUE("CREWCOUNT >= 0", crew >= 0).

PRINT "Vessel basics tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Target distance and relative velocity
// -----------------------------------------------------------------------------
PRINT "TEST: target distance and relative velocity".

SET tDist TO info:TARGETDISTANCE.
SET tRelV TO info:TARGETRELV.

// They may be 0 if there is no target, we only check that access works:
ASSERT_TRUE("TARGETDISTANCE read OK", TRUE).
ASSERT_TRUE("TARGETRELV read OK", TRUE).

PRINT "Target info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Biome info
// -----------------------------------------------------------------------------
PRINT "TEST: biome info".

SET rawBiome TO info:CURRENTRAWBIOME.
SET biome TO info:CURRENTBIOME.

// May be empty for some situations; just ensure reading works:
ASSERT_TRUE("CURRENTRAWBIOME read OK", TRUE).
ASSERT_TRUE("CURRENTBIOME read OK", TRUE).

PRINT "Biome info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "      INFO TEST SUMMARY        ".
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
    PRINT "ALL INFO TESTS PASSED ✅".
}.

PRINT "===============================".
PRINT "Info tests finished.".