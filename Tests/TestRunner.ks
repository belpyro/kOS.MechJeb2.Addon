// MJ_TestRunner.ks
// Menu runner for MechJeb kOS wrapper tests using Terminal:INPUT

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
    PARAMETER prompt IS "Enter choice (0–5): ".

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
    PRINT "Select test suite to run (press key 0–5):".
    PRINT "  1) Core wrapper tests".
    PRINT "  2) Vessel wrapper tests".
    PRINT "  3) Info wrapper tests".
    PRINT "  4) Ascent wrapper tests".
    PRINT "  5) Run ALL tests (1–4)".
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
        RUN CoreWrapperTest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "2" {
        PRINT "".
        PRINT "Running VesselWrapperTests...".
        RUN VesselWrapperTest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "3" {
        PRINT "".
        PRINT "Running InfoWrapperTests...".
        RUN InfoWrapperTest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "4" {
        PRINT "".
        PRINT "Running Ascent wrapper tests...".
        RUN AscentWrapperTest.
        WAIT_FOR_KEY().

    } ELSE IF choice = "5" {
        PRINT "".
        PRINT "Running ALL test suites...".

        PRINT "---------------- CORE ----------------".
        RUN CoreWrapperTest.
        WAIT_FOR_KEY().

        PRINT "---------------- VESSEL --------------".
        RUN VesselWrapperTest.
        WAIT_FOR_KEY().

        PRINT "---------------- INFO ----------------".
        RUN InfoWrapperTest.
        WAIT_FOR_KEY().

        PRINT "---------------- ASCENT --------------".
        RUN AscentWrapperTest.
        WAIT_FOR_KEY().

    } ELSE {
        PRINT "".
        PRINT "Unknown choice: " + choice + " (expected 0–5).".
        WAIT_FOR_KEY().
    }.
}.

PRINT "".
PRINT "MechJeb kOS Tests Runner finished.".