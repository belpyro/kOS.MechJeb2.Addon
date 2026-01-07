// ManeuverPlannerTimedTest.ks
// Unit tests for *TIMED suffix variants (X_FROM_NOW support)
// Simple inline tests - no function declarations

SET __MCP_SCRIPT_DONE__ TO FALSE.

CLEARSCREEN.
PRINT "===============================".
PRINT " ManeuverPlanner *TIMED TESTS  ".
PRINT "===============================".

SET planner TO ADDONS:MJ:PLANNER.
SET leadSec TO 30.
SET passed TO 0.
SET failed TO 0.

// Helper to clear nodes
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: CHANGEPETIMED
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEPETIMED".
SET res TO planner:CHANGEPETIMED(80000, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: CHANGEAPTIMED
// -----------------------------------------------------------------------------
PRINT "TEST: CHANGEAPTIMED".
SET res TO planner:CHANGEAPTIMED(150000, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: CIRCULARIZETIMED
// -----------------------------------------------------------------------------
PRINT "TEST: CIRCULARIZETIMED".
SET res TO planner:CIRCULARIZETIMED("X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: ELLIPTICIZETIMED
// -----------------------------------------------------------------------------
PRINT "TEST: ELLIPTICIZETIMED".
SET res TO planner:ELLIPTICIZETIMED(80000, 150000, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: SEMIMAJORTIMED
// -----------------------------------------------------------------------------
PRINT "TEST: SEMIMAJORTIMED".
SET bodySma TO BODY:RADIUS + 120000.
SET res TO planner:SEMIMAJORTIMED(bodySma, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: ECCENTRICITYTIMED
// -----------------------------------------------------------------------------
PRINT "TEST: ECCENTRICITYTIMED".
SET res TO planner:ECCENTRICITYTIMED(0.1, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// TEST: RESONANTORBITTIMED
// -----------------------------------------------------------------------------
PRINT "TEST: RESONANTORBITTIMED".
SET res TO planner:RESONANTORBITTIMED(2, 3, "X_FROM_NOW", leadSec).
WAIT 0.3.
IF res AND HASNODE AND ABS(NEXTNODE:ETA - leadSec) < 5 {
    PRINT "  PASS (ETA: " + ROUND(NEXTNODE:ETA, 1) + "s)".
    SET passed TO passed + 1.
} ELSE {
    PRINT "  FAIL: res=" + res + " hasnode=" + HASNODE.
    SET failed TO failed + 1.
}.
UNTIL NOT HASNODE { REMOVE NEXTNODE. WAIT 0.1. }.

// -----------------------------------------------------------------------------
// SUMMARY
// -----------------------------------------------------------------------------
PRINT "".
PRINT "===============================".
PRINT "Passed: " + passed + " / " + (passed + failed).
PRINT "Failed: " + failed.
PRINT "===============================".

SET __MCP_SCRIPT_DONE__ TO TRUE.
