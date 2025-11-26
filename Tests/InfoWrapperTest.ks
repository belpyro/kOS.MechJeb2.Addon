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
// Test: Maneuver / node
// -----------------------------------------------------------------------------
PRINT "TEST: maneuver / node info".

SET burnTime TO info:NEXTMANEUVERNODEBURNTIME.
SET nodeEta TO info:TIMETOMANEUVERNODE.
SET nodeDv TO info:NEXTMANEUVERNODEDELTAV.

// May be -1 if no node exists, just make sure we can read them:
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
// Test: Stage / total ΔV
// -----------------------------------------------------------------------------
PRINT "TEST: stage / total ΔV".

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
// Test: Target distance / relative motion
// -----------------------------------------------------------------------------
PRINT "TEST: target distance / relative motion".

SET tDist TO info:TARGETDISTANCE.
SET tRelV TO info:TARGETRELV.

ASSERT_TRUE("TARGETDISTANCE read OK", TRUE).
ASSERT_TRUE("TARGETRELV read OK", TRUE).

PRINT "Target info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Biomes
// -----------------------------------------------------------------------------
PRINT "TEST: biome info".

SET rawBiome TO info:CURRENTRAWBIOME.
SET biome TO info:CURRENTBIOME.

// May be empty if no biome data is available:
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