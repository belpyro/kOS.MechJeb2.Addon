// MJ_TestRunner.ks
// Menu runner for MechJeb kOS wrapper tests using Terminal:INPUT

// Ensure we're running from archive (where the test scripts are)
SWITCH TO 0.

CLEARSCREEN.
PRINT "===============================".
PRINT "   MechJeb kOS Tests Runner    ".
PRINT "===============================".
PRINT "".

// -----------------------------------------------------------------------------
// Shared helpers
// -----------------------------------------------------------------------------

// Wait for any key before continuing
DECLARE FUNCTION WAIT_FOR_KEY {
    LOCAL ti IS TERMINAL:INPUT.
    PRINT "".
    PRINT "Press any key to continue...".
    ti:CLEAR().
    LOCAL ch IS ti:GETCHAR().
}.

// Read a single-character menu choice using Terminal:INPUT
DECLARE FUNCTION READ_MENU_CHOICE {
    PARAMETER prompt IS "Enter choice: ".

    LOCAL ti IS TERMINAL:INPUT.
    LOCAL ch IS "".

    ti:CLEAR().
    PRINT prompt.

    SET ch TO ti:GETCHAR().
    PRINT ch.

    RETURN ch.
}

// -----------------------------------------------------------------------------
// Main loop
// -----------------------------------------------------------------------------

SET running TO TRUE.

UNTIL NOT running {

    PRINT "".
    PRINT "Select test suite to run:".
    PRINT "  1) Core wrapper tests".
    PRINT "  2) Vessel wrapper tests".
    PRINT "  3) Info wrapper tests".
    PRINT "  4) Ascent wrapper tests".
    PRINT "  5) Maneuver Planner wrapper tests".
    PRINT "  6) Basic maneuver tests".
    PRINT "  7) Orbital geometry tests".
    PRINT "  8) Rendezvous tests".
    PRINT "  9) Transfer tests".
    PRINT "  A) Node Executor wrapper tests".
    PRINT "  B) Run ALL tests".
    PRINT "  0) Exit".
    PRINT "".

    SET choice TO READ_MENU_CHOICE().

    IF choice = "0" {
        PRINT "".
        PRINT "Exiting test runner.".
        SET running TO FALSE.

    } ELSE IF choice = "1" {
        PRINT "".
        PRINT "Running CoreWrapperTests...".
        RUN corewrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "2" {
        PRINT "".
        PRINT "Running VesselWrapperTests...".
        RUN vesselwrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "3" {
        PRINT "".
        PRINT "Running InfoWrapperTests...".
        RUN infowrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "4" {
        PRINT "".
        PRINT "Running Ascent wrapper tests...".
        RUN ascentwrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "5" {
        PRINT "".
        PRINT "Running ManeuverPlanner wrapper tests...".
        RUN maneuverplannerwrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "6" {
        PRINT "".
        PRINT "Running Basic maneuver tests...".
        RUN maneuverplannerbasictest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "7" {
        PRINT "".
        PRINT "Running Orbital geometry tests...".
        RUN maneuverplannerorbitaltest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "8" {
        PRINT "".
        PRINT "Running Rendezvous tests...".
        RUN maneuverplannerrendezvoustest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "9" {
        PRINT "".
        PRINT "Running Transfer tests...".
        RUN maneuverplannertransfertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "A" OR choice = "a" {
        PRINT "".
        PRINT "Running NodeExecutor wrapper tests...".
        RUN nodeexecutorwrappertest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "B" OR choice = "b" {
        PRINT "".
        PRINT "Running ALL test suites...".

        PRINT "---------------- CORE ----------------".
        RUN corewrappertest.
        WAIT_FOR_KEY().

        PRINT "---------------- VESSEL --------------".
        RUN vesselwrappertest.
        WAIT_FOR_KEY().

        PRINT "---------------- INFO ----------------".
        RUN infowrappertest.
        WAIT_FOR_KEY().

        PRINT "---------------- ASCENT --------------".
        RUN ascentwrappertest.
        WAIT_FOR_KEY().

        PRINT "---------------- PLANNER -------------".
        RUN maneuverplannerwrappertest.
        WAIT_FOR_KEY().

        PRINT "---------------- BASIC ---------------".
        RUN maneuverplannerbasictest.
        WAIT_FOR_KEY().

        PRINT "---------------- ORBITAL -------------".
        RUN maneuverplannerorbitaltest.
        WAIT_FOR_KEY().

        PRINT "---------------- RENDEZVOUS ----------".
        RUN maneuverplannerrendezvoustest.
        WAIT_FOR_KEY().

        PRINT "---------------- TRANSFER ------------".
        RUN maneuverplannertransfertest.
        WAIT_FOR_KEY().

        PRINT "---------------- NODE ----------------".
        RUN nodeexecutorwrappertest.
        WAIT_FOR_KEY().

    } ELSE {
        PRINT "".
        PRINT "Unknown choice: " + choice + " (expected 0-9, A, B).".
        WAIT_FOR_KEY().
    }.
}.

PRINT "".
PRINT "MechJeb kOS Tests Runner finished.".
