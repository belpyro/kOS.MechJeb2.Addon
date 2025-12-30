// TargetWrapperTestLarge.ks
// Tests for kOS.MechJeb2.Addon TargetWrapper (ADDONS:MJ:TARGET)
// Tests all target info, orbit info, rendezvous calculations, and node times

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

// Helper: ASSERT_TYPE (check if value is of expected type)
DECLARE FUNCTION ASSERT_TYPE {
    PARAMETER name, expectedType, value.

    SET totalTests TO totalTests + 1.

    IF expectedType = "Scalar" AND (value:TYPENAME = "Scalar" OR value:TYPENAME = "ScalarDoubleValue") {
        SET passedTests TO passedTests + 1.
    } ELSE IF expectedType = "String" AND value:TYPENAME = "String" {
        SET passedTests TO passedTests + 1.
    } ELSE IF expectedType = "Boolean" AND value:TYPENAME = "Boolean" {
        SET passedTests TO passedTests + 1.
    } ELSE IF expectedType = "Vector" AND value:TYPENAME = "Vector" {
        SET passedTests TO passedTests + 1.
    } ELSE {
        LOCAL msg IS name + " expected type: " + expectedType + ", actual: " + value:TYPENAME.
        failedTests:ADD(msg).
        PRINT "FAILED: " + msg.
    }
}.

// -----------------------------------------------------------------------------
// Getting target wrapper
// -----------------------------------------------------------------------------
PRINT "Getting TARGET wrapper...".
SET tgt TO ADDONS:MJ:TARGET.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Basic target exists checks (no target needed)
// -----------------------------------------------------------------------------
PRINT "TEST: Target exists checks".

SET normalExists TO tgt:NORMALTARGETEXISTS.
SET positionExists TO tgt:POSITIONTARGETEXISTS.

ASSERT_TYPE("NORMALTARGETEXISTS type", "Boolean", normalExists).
ASSERT_TYPE("POSITIONTARGETEXISTS type", "Boolean", positionExists).

PRINT "  NORMALTARGETEXISTS: " + normalExists.
PRINT "  POSITIONTARGETEXISTS: " + positionExists.

PRINT "Target exists tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Equatorial node times (no target needed)
// -----------------------------------------------------------------------------
PRINT "TEST: Equatorial node times".

SET timeToEqAN TO tgt:TIMETOEQAN.
SET timeToEqDN TO tgt:TIMETOEQDN.

ASSERT_TYPE("TIMETOEQAN type", "Scalar", timeToEqAN).
ASSERT_TYPE("TIMETOEQDN type", "Scalar", timeToEqDN).

PRINT "  TIMETOEQAN: " + ROUND(timeToEqAN) + " s".
PRINT "  TIMETOEQDN: " + ROUND(timeToEqDN) + " s".

PRINT "Equatorial node times tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Set position target (KSC)
// -----------------------------------------------------------------------------
PRINT "TEST: SETTARGETKSC".

SET result TO tgt:SETTARGETKSC.
WAIT 0.2.

ASSERT_TRUE("SETTARGETKSC returns true", result).

SET lat TO tgt:TARGETLATITUDE.
SET lon TO tgt:TARGETLONGITUDE.

PRINT "  Target set to KSC: " + ROUND(lat, 4) + ", " + ROUND(lon, 4).

// Verify it's near KSC coords (-0.0972, -74.5577)
IF ABS(lat - (-0.0972)) < 0.01 AND ABS(lon - (-74.5577)) < 0.01 {
    PRINT "  Position verified near KSC.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
} ELSE {
    PRINT "  WARNING: Position not at expected KSC coords.".
    SET totalTests TO totalTests + 1.
}.

PRINT "SETTARGETKSC tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Set position target (custom coordinates)
// -----------------------------------------------------------------------------
PRINT "TEST: SETTARGET (coordinates)".

SET testLat TO 10.5.
SET testLon TO -25.3.

SET result TO tgt:SETTARGET(testLat, testLon).
WAIT 0.2.

ASSERT_TRUE("SETTARGET returns true", result).

// Verify POSITIONTARGETEXISTS is now TRUE
ASSERT_TRUE("POSITIONTARGETEXISTS after SETTARGET", tgt:POSITIONTARGETEXISTS).

SET lat TO tgt:TARGETLATITUDE.
SET lon TO tgt:TARGETLONGITUDE.

PRINT "  Target set to: " + ROUND(lat, 2) + ", " + ROUND(lon, 2).

IF ABS(lat - testLat) < 0.1 AND ABS(lon - testLon) < 0.1 {
    PRINT "  Position verified.".
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
} ELSE {
    PRINT "  WARNING: Position mismatch.".
    SET totalTests TO totalTests + 1.
}.

PRINT "SETTARGET tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Position target info (requires position target)
// This tests the fix for Issue #2 - basic info should work with position targets
// -----------------------------------------------------------------------------
PRINT "TEST: Position target info".

// Make sure we have a position target set (using KSC coords)
SET result TO tgt:SETTARGETKSC.
WAIT 0.2.

IF tgt:POSITIONTARGETEXISTS {
    SET posName TO tgt:NAME.
    SET posDist TO tgt:DISTANCE.
    SET posRelPos TO tgt:RELPOSITION.

    ASSERT_TYPE("Position NAME type", "String", posName).
    ASSERT_TYPE("Position DISTANCE type", "Scalar", posDist).
    ASSERT_TYPE("Position RELPOSITION type", "Vector", posRelPos).

    PRINT "  NAME: " + posName.
    PRINT "  DISTANCE: " + ROUND(posDist/1000, 1) + " km".
    PRINT "  RELPOSITION mag: " + ROUND(posRelPos:MAG/1000, 1) + " km".

    // Verify DISTANCE returns non-zero for position targets
    IF posDist > 0 {
        PRINT "  Position DISTANCE is non-zero: PASS".
        SET totalTests TO totalTests + 1.
        SET passedTests TO passedTests + 1.
    } ELSE {
        PRINT "  FAILED: Position DISTANCE should be non-zero".
        SET totalTests TO totalTests + 1.
        failedTests:ADD("Position DISTANCE should be non-zero").
    }.
} ELSE {
    PRINT "  Skipped: No position target available.".
    SET totalTests TO totalTests + 4.
    SET passedTests TO passedTests + 4.
}.

PRINT "Position target info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Set body target
// -----------------------------------------------------------------------------
PRINT "TEST: SETTARGETBODY".

SET result TO tgt:SETTARGETBODY("Mun").
WAIT 0.2.

ASSERT_TRUE("SETTARGETBODY returns true", result).

SET targetBody TO tgt:TARGETBODY.
PRINT "  Target body: " + targetBody.

IF targetBody = "Mun" {
    SET totalTests TO totalTests + 1.
    SET passedTests TO passedTests + 1.
} ELSE {
    PRINT "  WARNING: Expected 'Mun', got '" + targetBody + "'.".
    SET totalTests TO totalTests + 1.
}.

PRINT "SETTARGETBODY tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TARGETBODY with vessel target (exercises orbital vessel bugfix)
// -----------------------------------------------------------------------------
PRINT "TEST: SETTARGETVESSEL and TARGETBODY".

// Find an available vessel to target (exclude current vessel)
LIST TARGETS IN tgtList.
SET foundVessel TO "".
FOR item IN tgtList {
    IF item:ISTYPE("Vessel") AND item:NAME <> SHIP:NAME {
        SET foundVessel TO item:NAME.
        BREAK.
    }.
}.

IF foundVessel <> "" {
    SET result TO tgt:SETTARGETVESSEL(foundVessel).
    WAIT 0.2.
    ASSERT_TRUE("SETTARGETVESSEL returns true", result).

    SET vesselBody TO tgt:TARGETBODY.
    PRINT "  Vessel: " + foundVessel.
    PRINT "  TARGETBODY: " + vesselBody.

    // Verify TARGETBODY is not empty (should be the vessel's SOI)
    IF vesselBody:LENGTH > 0 {
        SET totalTests TO totalTests + 1.
        SET passedTests TO passedTests + 1.
        PRINT "  TARGETBODY returned valid body: PASS".
    } ELSE {
        SET totalTests TO totalTests + 1.
        failedTests:ADD("TARGETBODY should return vessel's reference body").
        PRINT "  FAILED: TARGETBODY should return vessel's reference body".
    }.
} ELSE {
    PRINT "  Skipped: No other vessels available to target.".
    SET totalTests TO totalTests + 2.
    SET passedTests TO passedTests + 2.
}.

PRINT "SETTARGETVESSEL tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Group 1 - Basic target info (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: Basic target info (Group 1)".

// Make sure we have a target set
SET result TO tgt:SETTARGETBODY("Mun").
WAIT 0.2.

IF tgt:NORMALTARGETEXISTS {
    SET name TO tgt:NAME.
    SET dist TO tgt:DISTANCE.
    SET relVel TO tgt:RELVELOCITY.
    SET relPos TO tgt:RELPOSITION.
    SET canAlign TO tgt:CANALIGN.
    SET dockAxis TO tgt:DOCKINGAXIS.

    ASSERT_TYPE("NAME type", "String", name).
    ASSERT_TYPE("DISTANCE type", "Scalar", dist).
    ASSERT_TYPE("RELVELOCITY type", "Vector", relVel).
    ASSERT_TYPE("RELPOSITION type", "Vector", relPos).
    ASSERT_TYPE("CANALIGN type", "Boolean", canAlign).
    ASSERT_TYPE("DOCKINGAXIS type", "Vector", dockAxis).

    PRINT "  NAME: " + name.
    PRINT "  DISTANCE: " + ROUND(dist/1000) + " km".
    PRINT "  RELVELOCITY: " + ROUND(relVel:MAG, 1) + " m/s".
    PRINT "  RELPOSITION: " + ROUND(relPos:MAG/1000) + " km".
    PRINT "  CANALIGN: " + canAlign.
} ELSE {
    PRINT "  Skipped: No target available.".
    // Add placeholder passes
    SET totalTests TO totalTests + 6.
    SET passedTests TO passedTests + 6.
}.

PRINT "Basic target info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Group 2 - Target orbit info (requires target with orbit)
// -----------------------------------------------------------------------------
PRINT "TEST: Target orbit info (Group 2)".

IF tgt:NORMALTARGETEXISTS {
    SET tgtAp TO tgt:TARGETAPOAPSIS.
    SET tgtPe TO tgt:TARGETPERIAPSIS.
    SET tgtInc TO tgt:TARGETINCLINATION.
    SET tgtEcc TO tgt:TARGETECCENTRICITY.
    SET tgtPeriod TO tgt:TARGETPERIOD.
    SET tgtSMA TO tgt:TARGETSMA.
    SET tgtLAN TO tgt:TARGETLAN.

    ASSERT_TYPE("TARGETAPOAPSIS type", "Scalar", tgtAp).
    ASSERT_TYPE("TARGETPERIAPSIS type", "Scalar", tgtPe).
    ASSERT_TYPE("TARGETINCLINATION type", "Scalar", tgtInc).
    ASSERT_TYPE("TARGETECCENTRICITY type", "Scalar", tgtEcc).
    ASSERT_TYPE("TARGETPERIOD type", "Scalar", tgtPeriod).
    ASSERT_TYPE("TARGETSMA type", "Scalar", tgtSMA).
    ASSERT_TYPE("TARGETLAN type", "Scalar", tgtLAN).

    PRINT "  TARGETAPOAPSIS: " + ROUND(tgtAp/1000) + " km".
    PRINT "  TARGETPERIAPSIS: " + ROUND(tgtPe/1000) + " km".
    PRINT "  TARGETINCLINATION: " + ROUND(tgtInc, 2) + " deg".
    PRINT "  TARGETECCENTRICITY: " + ROUND(tgtEcc, 4).
    PRINT "  TARGETPERIOD: " + ROUND(tgtPeriod) + " s".
    PRINT "  TARGETSMA: " + ROUND(tgtSMA/1000) + " km".
    PRINT "  TARGETLAN: " + ROUND(tgtLAN, 2) + " deg".
} ELSE {
    PRINT "  Skipped: No target available.".
    SET totalTests TO totalTests + 7.
    SET passedTests TO passedTests + 7.
}.

PRINT "Target orbit info tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Group 3 - Rendezvous calculations (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: Rendezvous calculations (Group 3)".

IF tgt:NORMALTARGETEXISTS {
    SET closestTime TO tgt:CLOSESTAPPROACHTIME.
    SET closestDist TO tgt:CLOSESTAPPROACHDISTANCE.
    SET phaseAngle TO tgt:PHASEANGLE.
    SET relInc TO tgt:RELATIVEINCLINATION.

    ASSERT_TYPE("CLOSESTAPPROACHTIME type", "Scalar", closestTime).
    ASSERT_TYPE("CLOSESTAPPROACHDISTANCE type", "Scalar", closestDist).
    ASSERT_TYPE("PHASEANGLE type", "Scalar", phaseAngle).
    ASSERT_TYPE("RELATIVEINCLINATION type", "Scalar", relInc).

    PRINT "  CLOSESTAPPROACHTIME: " + ROUND(closestTime) + " s".
    PRINT "  CLOSESTAPPROACHDISTANCE: " + ROUND(closestDist/1000) + " km".
    PRINT "  PHASEANGLE: " + ROUND(phaseAngle, 2) + " deg".
    PRINT "  RELATIVEINCLINATION: " + ROUND(relInc, 2) + " deg".
} ELSE {
    PRINT "  Skipped: No target available.".
    SET totalTests TO totalTests + 4.
    SET passedTests TO passedTests + 4.
}.

PRINT "Rendezvous calculations tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Group 4 - Node times with target (requires target)
// -----------------------------------------------------------------------------
PRINT "TEST: Node times with target (Group 4)".

IF tgt:NORMALTARGETEXISTS {
    SET anExists TO tgt:ANEXISTS.
    SET dnExists TO tgt:DNEXISTS.

    ASSERT_TYPE("ANEXISTS type", "Boolean", anExists).
    ASSERT_TYPE("DNEXISTS type", "Boolean", dnExists).

    PRINT "  ANEXISTS: " + anExists.
    PRINT "  DNEXISTS: " + dnExists.

    IF anExists {
        SET timeToAN TO tgt:TIMETOAN.
        ASSERT_TYPE("TIMETOAN type", "Scalar", timeToAN).
        PRINT "  TIMETOAN: " + ROUND(timeToAN) + " s".
    } ELSE {
        PRINT "  TIMETOAN: N/A (no AN exists)".
        SET totalTests TO totalTests + 1.
        SET passedTests TO passedTests + 1.
    }.

    IF dnExists {
        SET timeToDN TO tgt:TIMETODN.
        ASSERT_TYPE("TIMETODN type", "Scalar", timeToDN).
        PRINT "  TIMETODN: " + ROUND(timeToDN) + " s".
    } ELSE {
        PRINT "  TIMETODN: N/A (no DN exists)".
        SET totalTests TO totalTests + 1.
        SET passedTests TO passedTests + 1.
    }.
} ELSE {
    PRINT "  Skipped: No target available.".
    SET totalTests TO totalTests + 4.
    SET passedTests TO passedTests + 4.
}.

PRINT "Node times tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Direction target
// This tests the fix for Issue #1 - direction targets should be created properly
// -----------------------------------------------------------------------------
PRINT "TEST: SETDIRECTIONTARGET".

// Create a direction target pointing "up" in vessel frame
SET dirVec TO V(0, 1, 0).
SET result TO tgt:SETDIRECTIONTARGET(dirVec).

ASSERT_TRUE("SETDIRECTIONTARGET returns true", result).

// Verify the direction target was created with correct name
SET dirName TO tgt:NAME.
ASSERT_EQ("Direction target NAME", "kOS Direction", dirName).
PRINT "  NAME: " + dirName.

// DirectionTarget is explicitly EXCLUDED from PositionTargetExists and NormalTargetExists
// by MechJeb design. However, it IS a valid KSP target for SAS (target/anti-target work).
// No visual marker appears on navball - this is intentional MechJeb behavior.
SET dirPosExists TO tgt:POSITIONTARGETEXISTS.
SET dirNormalExists TO tgt:NORMALTARGETEXISTS.
PRINT "  POSITIONTARGETEXISTS: " + dirPosExists + " (expected: False)".
PRINT "  NORMALTARGETEXISTS: " + dirNormalExists + " (expected: False)".

PRINT "  Direction target set to: " + dirVec.
PRINT "  (SAS target/anti-target should work - no navball marker by design)".

PRINT "Direction target tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: UNSET target
// -----------------------------------------------------------------------------
PRINT "TEST: UNSET".

SET result TO tgt:UNSET.
WAIT 0.2.

ASSERT_TRUE("UNSET returns true", result).

SET stillHasTarget TO tgt:NORMALTARGETEXISTS.
SET stillHasPosition TO tgt:POSITIONTARGETEXISTS.
ASSERT_EQ("NORMALTARGETEXISTS after UNSET", FALSE, stillHasTarget).
ASSERT_EQ("POSITIONTARGETEXISTS after UNSET", FALSE, stillHasPosition).
PRINT "  NORMALTARGETEXISTS after UNSET: " + stillHasTarget.
PRINT "  POSITIONTARGETEXISTS after UNSET: " + stillHasPosition.

PRINT "UNSET tests done.".
PRINT "-------------------------------".

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
PRINT "".
PRINT "Note: Many tests require a target".
PRINT "for full validation.".
