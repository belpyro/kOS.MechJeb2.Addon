// AscentWrapperTests.ks
// Tests For kOS.MechJeb2.Addon AscentWrapper

CLEARSCREEN.
PRINT "===============================".
PRINT " MechJeb AscentWrapper TESTS   ".
PRINT "===============================".

// -----------------------------------------------------------------------------
// Global counters
// -----------------------------------------------------------------------------
SET totalTests TO 0.
SET passedTests TO 0.
SET failedTests TO LIST().

// Simple Assert
DECLARE FUNCTION ASSERT_EQ {
    parameter name, expected, actual.

    SET totalTests TO totalTests + 1.

    IF expected = actual {
        SET passedTests TO passedTests + 1.
    } ELSE {
        LOCAL msg IS name + " expected: " + expected + ", actual: " + actual.
        failedTests:ADD(msg).
        PRINT "FAILED: " + msg.
    }
}.

PRINT "Getting Ascent wrapper...".
SET asc TO ADDONS:MJ:CORE:ASCENT.
PRINT "OK.".
PRINT "".

// -----------------------------------------------------------------------------
// Saving initial values
// -----------------------------------------------------------------------------
SET origEnabled           TO asc:ENABLED.
SET origDesiredAlt        TO asc:DESIREDALTITUDE.
SET origTurnStartAlt      TO asc:TURNSTARTALTITUDE.
SET origTurnStartVel      TO asc:TURNSTARTVELOCITY.
SET origTurnEndAlt        TO asc:TURNENDALTITUDE.
SET origTurnEndAng        TO asc:TURNENDANGLE.
SET origTurnShapeExp      TO asc:TURNSHAPEEXPONENT.
SET origAutoPath          TO asc:AUTOPATH.
SET origAutostage         TO asc:AUTOSTAGE.
SET origAscentTypeString  TO asc:ASCENTTYPE.
SET origAutoDeployAntennas   TO asc:AUTODEPLOYANTENNAS.
SET origAutodeploySolarPanels TO asc:AUTODEPLOYSOLARPANELS.
SET origSkipCircularization  TO asc:SKIPCIRCULARIZATION.
SET origAutostageLimit TO asc:AUTOSTAGELIMIT.
SET origAutostagePreDelay TO asc:AUTOSTAGEPREDELAY.
SET origAutostagePostDelay TO asc:AUTOSTAGEPOSTDELAY.
SET origFairingMaxDynamicPressure TO asc:FAIRINGMAXDYNAMICPRESSURE.
SET origFairingMinAltitude TO asc:FAIRINGMINALTITUDE.
SET origFairingMaxAerothermalFlux TO asc:FAIRINGMAXAEROTHERMALFLUX.
SET origHotStaging TO asc:HOTSTAGING.
SET origHotStagingLeadTime TO asc:HOTSTAGINGLEADTIME.
SET origDropSolids TO asc:DROPSOLIDS.
SET origDropSolidsLeadTime TO asc:DROPSOLIDSLEADTIME.
SET origClampAutoStageThrustPct TO asc:CLAMPAUTOSTAGETHRUSTPCT.
SET origDesiredInclination TO asc:DESIREDINCLINATION.
SET origCorrectiveSteering TO asc:CORRECTIVESTEERING.
SET origCorrectiveSteeringGain TO asc:CORRECTIVESTEERINGGAIN.

SET origForceRoll TO asc:FORCEROLL.
SET origVerticalRoll TO asc:VERTICALROLL.
SET origTurnRoll TO asc:TURNROLL.
SET origRollAltitude TO asc:ROLLALTITUDE.

SET origLimitAoA TO asc:LIMITAOA.
SET origMaxAoA TO asc:MAXAOA.
SET origAoALimitFadeoutPressure TO asc:AOALIMITFADEOUTPRESSURE.
SET origLimitQa TO asc:LIMITQA.
SET origLimitQaEnabled TO asc:LIMITQAENABLED.
SET origLimitToPreventOverheats TO asc:LIMITTOPREVENTOVERHEATS.
SET origAutoWarp TO asc:AUTOWARP.

PRINT "Original values stored.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ENABLED
// -----------------------------------------------------------------------------
PRINT "TEST: ENABLED (on/off)".

SET asc:ENABLED TO TRUE.
WAIT 0.1.
ASSERT_EQ("ENABLED set TRUE", TRUE, asc:ENABLED).

SET asc:ENABLED TO FALSE.
WAIT 0.1.
 ASSERT_EQ("ENABLED set FALSE", FALSE, asc:ENABLED).

// restore original
SET asc:ENABLED TO origEnabled.
WAIT 0.1.
 ASSERT_EQ("ENABLED restored", origEnabled, asc:ENABLED).
PRINT "ENABLED tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: DESIREDALTITUDE / DSRALT
// -----------------------------------------------------------------------------
PRINT "TEST: DESIREDALTITUDE + alias DSRALT".

SET testAlt TO MAX(20000, origDesiredAlt + 10000).
SET asc:DESIREDALTITUDE TO testAlt.
WAIT 0.1.

 ASSERT_EQ("DESIREDALTITUDE set", testAlt, asc:DESIREDALTITUDE).
 ASSERT_EQ("DSRALT alias matches", asc:DESIREDALTITUDE, asc:DSRALT).

// restore
SET asc:DESIREDALTITUDE TO origDesiredAlt.
WAIT 0.1.
 ASSERT_EQ("DESIREDALTITUDE restored", origDesiredAlt, asc:DESIREDALTITUDE).
PRINT "DESIREDALTITUDE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TURNSTARTALTITUDE / STARTALT
// -----------------------------------------------------------------------------
PRINT "TEST: TURNSTARTALTITUDE + alias STARTALT".

SET newStartAlt TO MAX(1000, origTurnStartAlt + 500).
SET asc:TURNSTARTALTITUDE TO newStartAlt.
WAIT 0.1.

 ASSERT_EQ("TURNSTARTALTITUDE set", newStartAlt, asc:TURNSTARTALTITUDE).
 ASSERT_EQ("STARTALT alias matches", asc:TURNSTARTALTITUDE, asc:STARTALT).

// restore
SET asc:TURNSTARTALTITUDE TO origTurnStartAlt.
WAIT 0.1.
 ASSERT_EQ("TURNSTARTALTITUDE restored", origTurnStartAlt, asc:TURNSTARTALTITUDE).
PRINT "TURNSTARTALTITUDE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TURNSTARTVELOCITY / STARTV
// -----------------------------------------------------------------------------
PRINT "TEST: TURNSTARTVELOCITY + alias STARTV".

SET newStartV TO MAX(100, origTurnStartVel + 50).
SET asc:TURNSTARTVELOCITY TO newStartV.
WAIT 0.1.

 ASSERT_EQ("TURNSTARTVELOCITY set", newStartV, asc:TURNSTARTVELOCITY).
 ASSERT_EQ("STARTV alias matches", asc:TURNSTARTVELOCITY, asc:STARTV).

// restore
SET asc:TURNSTARTVELOCITY TO origTurnStartVel.
WAIT 0.1.
 ASSERT_EQ("TURNSTARTVELOCITY restored", origTurnStartVel, asc:TURNSTARTVELOCITY).
PRINT "TURNSTARTVELOCITY tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TURNENDALTITUDE / ENDALT
// -----------------------------------------------------------------------------
PRINT "TEST: TURNENDALTITUDE + alias ENDALT".

SET newEndAlt TO MAX(30000, origTurnEndAlt + 5000).
SET asc:TURNENDALTITUDE TO newEndAlt.
WAIT 0.1.

 ASSERT_EQ("TURNENDALTITUDE set", newEndAlt, asc:TURNENDALTITUDE).
 ASSERT_EQ("ENDALT alias matches", asc:TURNENDALTITUDE, asc:ENDALT).

// restore
SET asc:TURNENDALTITUDE TO origTurnEndAlt.
WAIT 0.1.
 ASSERT_EQ("TURNENDALTITUDE restored", origTurnEndAlt, asc:TURNENDALTITUDE).
PRINT "TURNENDALTITUDE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TURNENDANGLE / ENDANG
// -----------------------------------------------------------------------------
PRINT "TEST: TURNENDANGLE + alias ENDANG".

SET newEndAng TO MIN(85, origTurnEndAng + 5).
SET asc:TURNENDANGLE TO newEndAng.
WAIT 0.1.

 ASSERT_EQ("TURNENDANGLE set", newEndAng, asc:TURNENDANGLE).
 ASSERT_EQ("ENDANG alias matches", asc:TURNENDANGLE, asc:ENDANG).

// restore
SET asc:TURNENDANGLE TO origTurnEndAng.
WAIT 0.1.
 ASSERT_EQ("TURNENDANGLE restored", origTurnEndAng, asc:TURNENDANGLE).
PRINT "TURNENDANGLE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: TURNSHAPEEXPONENT / TSHAPEEXP + clamp
// -----------------------------------------------------------------------------
PRINT "TEST: TURNSHAPEEXPONENT + clamp + alias TSHAPEEXP".

// Use a value that is exactly representable (0.25)
SET newShape TO 0.25.
SET asc:TURNSHAPEEXPONENT TO newShape.
WAIT 0.1.

 ASSERT_EQ("TURNSHAPEEXPONENT set", newShape, asc:TURNSHAPEEXPONENT).
 ASSERT_EQ("TSHAPEEXP alias matches", asc:TURNSHAPEEXPONENT, asc:TSHAPEEXP).

// check clamping down (less than 0)
SET asc:TURNSHAPEEXPONENT TO -1.
WAIT 0.1.
 ASSERT_EQ("TURNSHAPEEXPONENT clamp low", 0, asc:TURNSHAPEEXPONENT).

// check clamping up (greater than 1)
SET asc:TURNSHAPEEXPONENT TO 2.
WAIT 0.1.
 ASSERT_EQ("TURNSHAPEEXPONENT clamp high", 1, asc:TURNSHAPEEXPONENT).

// restore original value
SET asc:TURNSHAPEEXPONENT TO origTurnShapeExp.
WAIT 0.1.
 ASSERT_EQ("TURNSHAPEEXPONENT restored", origTurnShapeExp, asc:TURNSHAPEEXPONENT).
PRINT "TURNSHAPEEXPONENT tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTOPATH
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOPATH toggle".

SET asc:AUTOPATH TO NOT origAutoPath.
WAIT 0.1.
 ASSERT_EQ("AUTOPATH toggled", NOT origAutoPath, asc:AUTOPATH).

// restore
SET asc:AUTOPATH TO origAutoPath.
WAIT 0.1.
 ASSERT_EQ("AUTOPATH restored", origAutoPath, asc:AUTOPATH).
PRINT "AUTOPATH tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTOSTAGE
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOSTAGE toggle".

SET asc:AUTOSTAGE TO NOT origAutostage.
WAIT 0.1.
 ASSERT_EQ("AUTOSTAGE toggled", NOT origAutostage, asc:AUTOSTAGE).

// restore
SET asc:AUTOSTAGE TO origAutostage.
WAIT 0.1.
 ASSERT_EQ("AUTOSTAGE restored", origAutostage, asc:AUTOSTAGE).
PRINT "AUTOSTAGE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: ASCENTTYPE / ASCTYPE (read-only)
// -----------------------------------------------------------------------------
PRINT "TEST: ASCENTTYPE / ASCTYPE (read-only)".

// currently AscentType == 0 => "CLASSIC" or "NOT SUPPORTED"
 ASSERT_EQ("ASCENTTYPE equals alias ASCTYPE", asc:ASCENTTYPE, asc:ASCTYPE).

// just verify that the current string is not empty
 ASSERT_EQ("ASCENTTYPE not empty", TRUE, (asc:ASCENTTYPE <> "")).

PRINT "ASCENTTYPE tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTODEPLOYANTENNAS / AUTODANT
// -----------------------------------------------------------------------------
PRINT "TEST: AUTODEPLOYANTENNAS + alias AUTODANT".

SET asc:AUTODEPLOYANTENNAS TO NOT origAutoDeployAntennas.
WAIT 0.1.

ASSERT_EQ("AUTODEPLOYANTENNAS toggled",
    NOT origAutoDeployAntennas,
    asc:AUTODEPLOYANTENNAS).

ASSERT_EQ("AUTODANT alias matches",
    asc:AUTODEPLOYANTENNAS,
    asc:AUTODANT).

// restore
SET asc:AUTODEPLOYANTENNAS TO origAutoDeployAntennas.
WAIT 0.1.

ASSERT_EQ("AUTODEPLOYANTENNAS restored",
    origAutoDeployAntennas,
    asc:AUTODEPLOYANTENNAS).

PRINT "AUTODEPLOYANTENNAS tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTODEPLOYSOLARPANELS / AUTODSOL
// -----------------------------------------------------------------------------
PRINT "TEST: AUTODEPLOYSOLARPANELS + alias AUTODSOL".

SET asc:AUTODEPLOYSOLARPANELS TO NOT origAutodeploySolarPanels.
WAIT 0.1.

ASSERT_EQ("AUTODEPLOYSOLARPANELS toggled",
    NOT origAutodeploySolarPanels,
    asc:AUTODEPLOYSOLARPANELS).

ASSERT_EQ("AUTODSOL alias matches",
    asc:AUTODEPLOYSOLARPANELS,
    asc:AUTODSOL).

// restore
SET asc:AUTODEPLOYSOLARPANELS TO origAutodeploySolarPanels.
WAIT 0.1.

ASSERT_EQ("AUTODEPLOYSOLARPANELS restored",
    origAutodeploySolarPanels,
    asc:AUTODEPLOYSOLARPANELS).

PRINT "AUTODEPLOYSOLARPANELS tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: SKIPCIRCULARIZATION / SKIPCIRC
// -----------------------------------------------------------------------------
PRINT "TEST: SKIPCIRCULARIZATION + alias SKIPCIRC".

SET asc:SKIPCIRCULARIZATION TO NOT origSkipCircularization.
WAIT 0.1.

ASSERT_EQ("SKIPCIRCULARIZATION toggled",
    NOT origSkipCircularization,
    asc:SKIPCIRCULARIZATION).

ASSERT_EQ("SKIPCIRC alias matches",
    asc:SKIPCIRCULARIZATION,
    asc:SKIPCIRC).

// restore
SET asc:SKIPCIRCULARIZATION TO origSkipCircularization.
WAIT 0.1.

ASSERT_EQ("SKIPCIRCULARIZATION restored",
    origSkipCircularization,
    asc:SKIPCIRCULARIZATION).

PRINT "SKIPCIRCULARIZATION tests done.".
PRINT "-------------------------------".

// -----------------------------------------------------------------------------
// Test: AUTOSTAGELIMIT / ASTGLIM
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOSTAGELIMIT + alias ASTGLIM".

SET newLimit TO MAX(1, origAutostageLimit + 1).
SET asc:AUTOSTAGELIMIT TO newLimit.
WAIT 0.1.

ASSERT_EQ("AUTOSTAGELIMIT set", newLimit, asc:AUTOSTAGELIMIT).
ASSERT_EQ("ASTGLIM alias matches", asc:AUTOSTAGELIMIT, asc:ASTGLIM).

SET asc:AUTOSTAGELIMIT TO origAutostageLimit.
WAIT 0.1.

ASSERT_EQ("AUTOSTAGELIMIT restored", origAutostageLimit, asc:AUTOSTAGELIMIT).

PRINT "AUTOSTAGELIMIT tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: AUTOSTAGEPREDELAY / AUTOSTAGEPOSTDELAY
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOSTAGEPREDELAY / AUTOSTAGEPOSTDELAY".

SET newPreDelay TO origAutostagePreDelay + 0.5.
SET asc:AUTOSTAGEPREDELAY TO newPreDelay.
WAIT 0.1.

ASSERT_EQ("AUTOSTAGEPREDELAY set", newPreDelay, asc:AUTOSTAGEPREDELAY).

SET newPostDelay TO origAutostagePostDelay + 0.5.
SET asc:AUTOSTAGEPOSTDELAY TO newPostDelay.
WAIT 0.1.

ASSERT_EQ("AUTOSTAGEPOSTDELAY set", newPostDelay, asc:AUTOSTAGEPOSTDELAY).

SET asc:AUTOSTAGEPREDELAY TO origAutostagePreDelay.
SET asc:AUTOSTAGEPOSTDELAY TO origAutostagePostDelay.
WAIT 0.1.

ASSERT_EQ("AUTOSTAGEPREDELAY restored", origAutostagePreDelay, asc:AUTOSTAGEPREDELAY).
ASSERT_EQ("AUTOSTAGEPOSTDELAY restored", origAutostagePostDelay, asc:AUTOSTAGEPOSTDELAY).

PRINT "AUTOSTAGE PRE/POST DELAY tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: FAIRING jettison conditions
// -----------------------------------------------------------------------------
PRINT "TEST: FAIRING jettison conditions".

SET newMaxQ TO origFairingMaxDynamicPressure + 1000.
SET asc:FAIRINGMAXDYNAMICPRESSURE TO newMaxQ.
WAIT 0.1.

ASSERT_EQ("FAIRINGMAXDYNAMICPRESSURE set", newMaxQ, asc:FAIRINGMAXDYNAMICPRESSURE).

SET newMinAlt TO origFairingMinAltitude + 1000.
SET asc:FAIRINGMINALTITUDE TO newMinAlt.
WAIT 0.1.

ASSERT_EQ("FAIRINGMINALTITUDE set", newMinAlt, asc:FAIRINGMINALTITUDE).

SET newMaxFlux TO origFairingMaxAerothermalFlux + 10000.
SET asc:FAIRINGMAXAEROTHERMALFLUX TO newMaxFlux.
WAIT 0.1.

ASSERT_EQ("FAIRINGMAXAEROTHERMALFLUX set", newMaxFlux, asc:FAIRINGMAXAEROTHERMALFLUX).

SET asc:FAIRINGMAXDYNAMICPRESSURE TO origFairingMaxDynamicPressure.
SET asc:FAIRINGMINALTITUDE TO origFairingMinAltitude.
SET asc:FAIRINGMAXAEROTHERMALFLUX TO origFairingMaxAerothermalFlux.
WAIT 0.1.

ASSERT_EQ("FAIRINGMAXDYNAMICPRESSURE restored", origFairingMaxDynamicPressure, asc:FAIRINGMAXDYNAMICPRESSURE).
ASSERT_EQ("FAIRINGMINALTITUDE restored", origFairingMinAltitude, asc:FAIRINGMINALTITUDE).
ASSERT_EQ("FAIRINGMAXAEROTHERMALFLUX restored", origFairingMaxAerothermalFlux, asc:FAIRINGMAXAEROTHERMALFLUX).

PRINT "FAIRING jettison tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: HOTSTAGING / HOTSTAGINGLEADTIME
// -----------------------------------------------------------------------------
PRINT "TEST: HOTSTAGING + HOTSTAGINGLEADTIME".

SET asc:HOTSTAGING TO NOT origHotStaging.
WAIT 0.1.

ASSERT_EQ("HOTSTAGING toggled", NOT origHotStaging, asc:HOTSTAGING).

SET newHotLead TO origHotStagingLeadTime + 0.5.
SET asc:HOTSTAGINGLEADTIME TO newHotLead.
WAIT 0.1.

ASSERT_EQ("HOTSTAGINGLEADTIME set", newHotLead, asc:HOTSTAGINGLEADTIME).

SET asc:HOTSTAGING TO origHotStaging.
SET asc:HOTSTAGINGLEADTIME TO origHotStagingLeadTime.
WAIT 0.1.

ASSERT_EQ("HOTSTAGING restored", origHotStaging, asc:HOTSTAGING).
ASSERT_EQ("HOTSTAGINGLEADTIME restored", origHotStagingLeadTime, asc:HOTSTAGINGLEADTIME).

PRINT "HOTSTAGING tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: DROPSOLIDS / DROPSOLIDSLEADTIME
// -----------------------------------------------------------------------------
PRINT "TEST: DROPSOLIDS + DROPSOLIDSLEADTIME".

SET asc:DROPSOLIDS TO NOT origDropSolids.
WAIT 0.1.

ASSERT_EQ("DROPSOLIDS toggled", NOT origDropSolids, asc:DROPSOLIDS).

SET newDropLead TO origDropSolidsLeadTime + 0.5.
SET asc:DROPSOLIDSLEADTIME TO newDropLead.
WAIT 0.1.

ASSERT_EQ("DROPSOLIDSLEADTIME set", newDropLead, asc:DROPSOLIDSLEADTIME).

SET asc:DROPSOLIDS TO origDropSolids.
SET asc:DROPSOLIDSLEADTIME TO origDropSolidsLeadTime.
WAIT 0.1.

ASSERT_EQ("DROPSOLIDS restored", origDropSolids, asc:DROPSOLIDS).
ASSERT_EQ("DROPSOLIDSLEADTIME restored", origDropSolidsLeadTime, asc:DROPSOLIDSLEADTIME).

PRINT "DROPSOLIDS tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: CLAMPAUTOSTAGETHRUSTPCT / CLAMPTHRUST (clamp 0..1)
// -----------------------------------------------------------------------------
PRINT "TEST: CLAMPAUTOSTAGETHRUSTPCT + alias CLAMPTHRUST".

SET asc:CLAMPAUTOSTAGETHRUSTPCT TO 0.5.
WAIT 0.1.

ASSERT_EQ("CLAMPAUTOSTAGETHRUSTPCT set mid", 0.5, asc:CLAMPAUTOSTAGETHRUSTPCT).
ASSERT_EQ("CLAMPTHRUST alias matches", asc:CLAMPAUTOSTAGETHRUSTPCT, asc:CLAMPTHRUST).

SET asc:CLAMPAUTOSTAGETHRUSTPCT TO -1.
WAIT 0.1.

ASSERT_EQ("CLAMPAUTOSTAGETHRUSTPCT clamp low", 0, asc:CLAMPAUTOSTAGETHRUSTPCT).

SET asc:CLAMPAUTOSTAGETHRUSTPCT TO 2.
WAIT 0.1.

ASSERT_EQ("CLAMPAUTOSTAGETHRUSTPCT clamp high", 1, asc:CLAMPAUTOSTAGETHRUSTPCT).

SET asc:CLAMPAUTOSTAGETHRUSTPCT TO origClampAutoStageThrustPct.
WAIT 0.1.

ASSERT_EQ("CLAMPAUTOSTAGETHRUSTPCT restored", origClampAutoStageThrustPct, asc:CLAMPAUTOSTAGETHRUSTPCT).

PRINT "CLAMPAUTOSTAGETHRUSTPCT tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: DESIREDINCLINATION / INC
// -----------------------------------------------------------------------------
PRINT "TEST: DESIREDINCLINATION + alias INC".

SET newInc TO origDesiredInclination + 1.
SET asc:DESIREDINCLINATION TO newInc.
WAIT 0.1.

ASSERT_EQ("DESIREDINCLINATION set", newInc, asc:DESIREDINCLINATION).
ASSERT_EQ("INC alias matches", asc:DESIREDINCLINATION, asc:INC).

SET asc:DESIREDINCLINATION TO origDesiredInclination.
WAIT 0.1.

ASSERT_EQ("DESIREDINCLINATION restored", origDesiredInclination, asc:DESIREDINCLINATION).

PRINT "DESIREDINCLINATION tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: CORRECTIVESTEERING / CSTEER
// -----------------------------------------------------------------------------
PRINT "TEST: CORRECTIVESTEERING + alias CSTEER".

SET asc:CORRECTIVESTEERING TO NOT origCorrectiveSteering.
WAIT 0.1.

ASSERT_EQ("CORRECTIVESTEERING toggled", NOT origCorrectiveSteering, asc:CORRECTIVESTEERING).
ASSERT_EQ("CSTEER alias matches", asc:CORRECTIVESTEERING, asc:CSTEER).

SET asc:CORRECTIVESTEERING TO origCorrectiveSteering.
WAIT 0.1.

ASSERT_EQ("CORRECTIVESTEERING restored", origCorrectiveSteering, asc:CORRECTIVESTEERING).

PRINT "CORRECTIVESTEERING tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: CORRECTIVESTEERINGGAIN / CSTEERGAIN
// -----------------------------------------------------------------------------
PRINT "TEST: CORRECTIVESTEERINGGAIN + alias CSTEERGAIN".

SET newGain TO origCorrectiveSteeringGain + 0.1.
SET asc:CORRECTIVESTEERINGGAIN TO newGain.
WAIT 0.1.

ASSERT_EQ("CORRECTIVESTEERINGGAIN set", newGain, asc:CORRECTIVESTEERINGGAIN).
ASSERT_EQ("CSTEERGAIN alias matches", asc:CORRECTIVESTEERINGGAIN, asc:CSTEERGAIN).

SET asc:CORRECTIVESTEERINGGAIN TO origCorrectiveSteeringGain.
WAIT 0.1.

ASSERT_EQ("CORRECTIVESTEERINGGAIN restored", origCorrectiveSteeringGain, asc:CORRECTIVESTEERINGGAIN).

PRINT "CORRECTIVESTEERINGGAIN tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: FORCEROLL / FROLL
// -----------------------------------------------------------------------------
PRINT "TEST: FORCEROLL + alias FROLL".

SET asc:FORCEROLL TO NOT origForceRoll.
WAIT 0.1.

ASSERT_EQ("FORCEROLL toggled", NOT origForceRoll, asc:FORCEROLL).
ASSERT_EQ("FROLL alias matches", asc:FORCEROLL, asc:FROLL).

SET asc:FORCEROLL TO origForceRoll.
WAIT 0.1.

ASSERT_EQ("FORCEROLL restored", origForceRoll, asc:FORCEROLL).

PRINT "FORCEROLL tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: VERTICALROLL / VROLL
// -----------------------------------------------------------------------------
PRINT "TEST: VERTICALROLL + alias VROLL".

SET newVRoll TO origVerticalRoll + 5.
SET asc:VERTICALROLL TO newVRoll.
WAIT 0.1.

ASSERT_EQ("VERTICALROLL set", newVRoll, asc:VERTICALROLL).
ASSERT_EQ("VROLL alias matches", asc:VERTICALROLL, asc:VROLL).

SET asc:VERTICALROLL TO origVerticalRoll.
WAIT 0.1.

ASSERT_EQ("VERTICALROLL restored", origVerticalRoll, asc:VERTICALROLL).

PRINT "VERTICALROLL tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: TURNROLL / TROLL
// -----------------------------------------------------------------------------
PRINT "TEST: TURNROLL + alias TROLL".

SET newTRoll TO origTurnRoll + 5.
SET asc:TURNROLL TO newTRoll.
WAIT 0.1.

ASSERT_EQ("TURNROLL set", newTRoll, asc:TURNROLL).
ASSERT_EQ("TROLL alias matches", asc:TURNROLL, asc:TROLL).

SET asc:TURNROLL TO origTurnRoll.
WAIT 0.1.

ASSERT_EQ("TURNROLL restored", origTurnRoll, asc:TURNROLL).

PRINT "TURNROLL tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: ROLLALTITUDE / ROLLALT
// -----------------------------------------------------------------------------
PRINT "TEST: ROLLALTITUDE + alias ROLLALT".

SET newRollAlt TO origRollAltitude + 1000.
SET asc:ROLLALTITUDE TO newRollAlt.
WAIT 0.1.

ASSERT_EQ("ROLLALTITUDE set", newRollAlt, asc:ROLLALTITUDE).
ASSERT_EQ("ROLLALT alias matches", asc:ROLLALTITUDE, asc:ROLLALT).

SET asc:ROLLALTITUDE TO origRollAltitude.
WAIT 0.1.

ASSERT_EQ("ROLLALTITUDE restored", origRollAltitude, asc:ROLLALTITUDE).

PRINT "ROLLALTITUDE tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: LIMITAOA / LIMAOA
// -----------------------------------------------------------------------------
PRINT "TEST: LIMITAOA + alias LIMAOA".

SET asc:LIMITAOA TO NOT origLimitAoA.
WAIT 0.1.

ASSERT_EQ("LIMITAOA toggled", NOT origLimitAoA, asc:LIMITAOA).
ASSERT_EQ("LIMAOA alias matches", asc:LIMITAOA, asc:LIMAOA).

SET asc:LIMITAOA TO origLimitAoA.
WAIT 0.1.

ASSERT_EQ("LIMITAOA restored", origLimitAoA, asc:LIMITAOA).

PRINT "LIMITAOA tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: MAXAOA
// -----------------------------------------------------------------------------
PRINT "TEST: MAXAOA".

SET newMaxAoA TO origMaxAoA + 5.
SET asc:MAXAOA TO newMaxAoA.
WAIT 0.1.

ASSERT_EQ("MAXAOA set", newMaxAoA, asc:MAXAOA).

SET asc:MAXAOA TO origMaxAoA.
WAIT 0.1.

ASSERT_EQ("MAXAOA restored", origMaxAoA, asc:MAXAOA).

PRINT "MAXAOA tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: AOALIMITFADEOUTPRESSURE / AOAFADEQ
// -----------------------------------------------------------------------------
PRINT "TEST: AOALIMITFADEOUTPRESSURE + alias AOAFADEQ".

SET newFadeQ TO origAoALimitFadeoutPressure + 1000.
SET asc:AOALIMITFADEOUTPRESSURE TO newFadeQ.
WAIT 0.1.

ASSERT_EQ("AOALIMITFADEOUTPRESSURE set", newFadeQ, asc:AOALIMITFADEOUTPRESSURE).
ASSERT_EQ("AOAFADEQ alias matches", asc:AOALIMITFADEOUTPRESSURE, asc:AOAFADEQ).

SET asc:AOALIMITFADEOUTPRESSURE TO origAoALimitFadeoutPressure.
WAIT 0.1.

ASSERT_EQ("AOALIMITFADEOUTPRESSURE restored", origAoALimitFadeoutPressure, asc:AOALIMITFADEOUTPRESSURE).

PRINT "AOALIMITFADEOUTPRESSURE tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: LIMITQA / LIMQA and LIMITQAENABLED / LIMQAEN
// -----------------------------------------------------------------------------
PRINT "TEST: LIMITQA + LIMITQAENABLED".

SET newLimitQa TO origLimitQa + 5000.
SET asc:LIMITQA TO newLimitQa.
WAIT 0.1.

ASSERT_EQ("LIMITQA set", newLimitQa, asc:LIMITQA).
ASSERT_EQ("LIMQA alias matches", asc:LIMITQA, asc:LIMQA).

SET asc:LIMITQAENABLED TO NOT origLimitQaEnabled.
WAIT 0.1.

ASSERT_EQ("LIMITQAENABLED toggled", NOT origLimitQaEnabled, asc:LIMITQAENABLED).
ASSERT_EQ("LIMQAEN alias matches", asc:LIMITQAENABLED, asc:LIMQAEN).

SET asc:LIMITQA TO origLimitQa.
SET asc:LIMITQAENABLED TO origLimitQaEnabled.
WAIT 0.1.

ASSERT_EQ("LIMITQA restored", origLimitQa, asc:LIMITQA).
ASSERT_EQ("LIMITQAENABLED restored", origLimitQaEnabled, asc:LIMITQAENABLED).

PRINT "LIMITQA tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: LIMITTOPREVENTOVERHEATS / LIMOVHT
// -----------------------------------------------------------------------------
PRINT "TEST: LIMITTOPREVENTOVERHEATS + alias LIMOVHT".

SET asc:LIMITTOPREVENTOVERHEATS TO NOT origLimitToPreventOverheats.
WAIT 0.1.

ASSERT_EQ("LIMITTOPREVENTOVERHEATS toggled",
    NOT origLimitToPreventOverheats,
    asc:LIMITTOPREVENTOVERHEATS).

ASSERT_EQ("LIMOVHT alias matches",
    asc:LIMITTOPREVENTOVERHEATS,
    asc:LIMOVHT).

SET asc:LIMITTOPREVENTOVERHEATS TO origLimitToPreventOverheats.
WAIT 0.1.

ASSERT_EQ("LIMITTOPREVENTOVERHEATS restored",
    origLimitToPreventOverheats,
    asc:LIMITTOPREVENTOVERHEATS).

PRINT "LIMITTOPREVENTOVERHEATS tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// Test: AUTOWARP / AWARP
// -----------------------------------------------------------------------------
PRINT "TEST: AUTOWARP + alias AWARP".

SET asc:AUTOWARP TO NOT origAutoWarp.
WAIT 0.1.

ASSERT_EQ("AUTOWARP toggled",
    NOT origAutoWarp,
    asc:AUTOWARP).

ASSERT_EQ("AWARP alias matches",
    asc:AUTOWARP,
    asc:AWARP).

SET asc:AUTOWARP TO origAutoWarp.
WAIT 0.1.

ASSERT_EQ("AUTOWARP restored",
    origAutoWarp,
    asc:AUTOWARP).

PRINT "AUTOWARP tests done.".
PRINT "-------------------------------".
// -----------------------------------------------------------------------------
// FINAL DATA
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "          TEST SUMMARY         ".
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
    PRINT "ALL TESTS PASSED ✅".
}.

PRINT "===============================".
PRINT "Tests finished.".