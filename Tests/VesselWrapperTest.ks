// VesselWrapperTests.ks
// Tests for kOS.MechJeb2.Addon VesselStateWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT "  MechJeb VesselWrapper TESTS  ".
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
// Getting vessel wrapper
// -----------------------------------------------------------------------------
PRINT "Getting Vessel wrapper...".
SET v TO ADDONS:MJ:CORE:VESSEL.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Time & gravity
// -----------------------------------------------------------------------------
PRINT "TEST: TIME / LOCALG".

SET t TO v:TIME.
SET g TO v:LOCALG.

ASSERT_TRUE("TIME is non-negative", t >= 0).
ASSERT_TRUE("LOCALG is non-negative", g >= 0).

PRINT "TIME / LOCALG tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Velocities
// -----------------------------------------------------------------------------
PRINT "TEST: velocities (SPEEDSURFACE, SPEEDVERTICAL, SURFHVEL)".

SET vSurf TO v:SPEEDSURFACE.
SET vVert TO v:SPEEDVERTICAL.
SET vHorizSurf TO v:SPEEDSURFACEHORIZONTAL.

ASSERT_TRUE("SPEEDSURFACE >= 0", vSurf >= 0).
ASSERT_TRUE("abs(SPEEDVERTICAL) < 20000", ABS(vVert) < 20000).
ASSERT_TRUE("SURFHVEL >= 0", vHorizSurf >= 0).

PRINT "Velocity tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Attitude
// -----------------------------------------------------------------------------
PRINT "TEST: attitude (HEADING/PITCH/ROLL)".

SET hdg TO v:VESSELHEADING.
SET pitch TO v:VESSELPITCH.
SET roll TO v:VESSELROLL.

ASSERT_TRUE("HEADING in [0,360)", hdg >= 0 AND hdg < 360).
ASSERT_TRUE("PITCH in [-90,90]", pitch >= -90 AND pitch <= 90).
ASSERT_TRUE("ROLL in [-180,180]", roll >= -180 AND roll <= 180).

PRINT "Attitude tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Altitudes
// -----------------------------------------------------------------------------
PRINT "TEST: altitudes (ALTASL / ALTTRUE / SURFALT)".

SET altAsl TO v:ALTITUDEASL.
SET altTrue TO v:ALTITUDETRUE.
SET surfAlt TO v:SURFACEALTITUDEASL.

ASSERT_TRUE("ALTITUDEASL >= 0", altAsl >= 0).
ASSERT_TRUE("ALTITUDETRUE >= 0", altTrue >= 0).
ASSERT_TRUE("SURFACEALTITUDEASL >= 0", surfAlt >= 0).

PRINT "Altitude tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Orbit basics
// -----------------------------------------------------------------------------
PRINT "TEST: orbit basic access".

SET apa TO v:ORBITAPA.
SET pea TO v:ORBITPEA.
SET sma TO v:ORBITSEMIAXIS.

// just making sure access does not throw:
ASSERT_TRUE("ORBITAPA read OK", TRUE).
ASSERT_TRUE("ORBITPEA read OK", TRUE).
ASSERT_TRUE("ORBITSEMIAXIS read OK", TRUE).

PRINT "Orbit basic tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Aero angles and dynamic pressure
// -----------------------------------------------------------------------------
PRINT "TEST: AOA / AOS / Q / MACH".

SET aoa TO v:AOA.
SET aos TO v:AOS.
SET qdyn TO v:DYNAMICPRESSURE.
SET mach TO v:MACH.

ASSERT_TRUE("AOA in sane range", ABS(aoa) < 180).
ASSERT_TRUE("AOS in sane range", ABS(aos) < 180).
ASSERT_TRUE("Q >= 0", qdyn >= 0).
ASSERT_TRUE("MACH >= 0", mach >= 0).

PRINT "Aero / Q / Mach tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Net forces
// -----------------------------------------------------------------------------
PRINT "TEST: net forces (PUREDRAG / PURELIFT)".

SET dragF TO v:PUREDRAG.
SET liftF TO v:PURELIFT.

ASSERT_TRUE("PUREDRAG >= 0", dragF >= 0).
ASSERT_TRUE("PURELIFT >= 0", liftF >= 0).

PRINT "Net forces tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// FINAL SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "     VESSEL TEST SUMMARY       ".
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
    PRINT "ALL VESSEL TESTS PASSED ✅".
}.

PRINT "===============================".
PRINT "Vessel tests finished.".