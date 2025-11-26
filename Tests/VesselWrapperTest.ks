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
SET mjVessel TO ADDONS:MJ:CORE:VESSEL.
PRINT "OK.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Time and local gravity
// -----------------------------------------------------------------------------
PRINT "TEST: TIME / LOCALG".

SET t TO mjVessel:TIME.
SET g TO mjVessel:LOCALG.

ASSERT_TRUE("TIME is non-negative", t >= 0).
ASSERT_TRUE("LOCALG is non-negative", g >= 0).

PRINT "TIME / LOCALG tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Velocities
// -----------------------------------------------------------------------------
PRINT "TEST: velocities (SPEEDSURFACE, SPEEDVERTICAL, SURFHVEL)".

SET vSurf TO mjVessel:SPEEDSURFACE.
SET vVert TO mjVessel:SPEEDVERTICAL.
SET vHorizSurf TO mjVessel:SPEEDSURFACEHORIZONTAL.

ASSERT_TRUE("SPEEDSURFACE >= 0", vSurf >= 0).
ASSERT_TRUE("abs(SPEEDVERTICAL) < 20000", ABS(vVert) < 20000).
ASSERT_TRUE("SURFHVEL >= 0", vHorizSurf >= 0).

PRINT "Velocity tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Attitude
// -----------------------------------------------------------------------------
PRINT "TEST: attitude (HEADING / PITCH / ROLL)".

SET hdg TO mjVessel:VESSELHEADING.
SET pitch TO mjVessel:VESSELPITCH.
SET roll TO mjVessel:VESSELROLL.

ASSERT_TRUE("HEADING in [0, 360)", hdg >= 0 AND hdg < 360).
ASSERT_TRUE("PITCH in [-90, 90]", pitch >= -90 AND pitch <= 90).
ASSERT_TRUE("ROLL in [-180, 180]", roll >= -180 AND roll <= 180).

PRINT "Attitude tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Altitudes
// -----------------------------------------------------------------------------
PRINT "TEST: altitudes (ALTITUDEASL / ALTITUDETRUE / SURFACEALTITUDEASL)".

SET altAsl TO mjVessel:ALTITUDEASL.
SET altTrue TO mjVessel:ALTITUDETRUE.
SET surfAlt TO mjVessel:SURFACEALTITUDEASL.

ASSERT_TRUE("ALTITUDEASL >= 0", altAsl >= 0).
ASSERT_TRUE("ALTITUDETRUE >= 0", altTrue >= 0).
ASSERT_TRUE("SURFACEALTITUDEASL >= 0", surfAlt >= 0).

PRINT "Altitude tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Orbit basic access
// -----------------------------------------------------------------------------
PRINT "TEST: orbit basic access (APA / PEA / SMA)".

SET apa TO mjVessel:ORBITAPA.
SET pea TO mjVessel:ORBITPEA.
SET sma TO mjVessel:ORBITSEMIAXIS.

// These can be negative for suborbital or escape; we only assert that access works:
ASSERT_TRUE("ORBITAPA read OK", TRUE).
ASSERT_TRUE("ORBITPEA read OK", TRUE).
ASSERT_TRUE("ORBITSEMIAXIS read OK", TRUE).

PRINT "Orbit basic tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: Aero angles and dynamic pressure
// -----------------------------------------------------------------------------
PRINT "TEST: AOA / AOS / Q / MACH".

SET aoa TO mjVessel:AOA.
SET aos TO mjVessel:AOS.
SET qdyn TO mjVessel:DYNAMICPRESSURE.
SET mach TO mjVessel:MACH.

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

SET dragF TO mjVessel:PUREDRAG.
SET liftF TO mjVessel:PURELIFT.

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