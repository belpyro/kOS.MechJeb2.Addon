#!/usr/bin/env osascript

(*
Quick KSP save reloader (assumes KSP already running)
Just brings KSP to foreground and loads specified save
*)

--------------------------------------------------------
-- Helper functions
--------------------------------------------------------
on pressKey(keyChar)
    tell application "System Events" to keystroke keyChar
    delay 0.5
end pressKey

on pressSpecial(vKey)
    tell application "System Events" to key code vKey
    delay 1
end pressSpecial

on run argv
    main(argv)
end run

on main(argv)

log "📂 Loading KSP save (quick reload, no full restart)"
delay 1

--------------------------------------------------------
-- Get save name from arguments (default: test-in-orbit)
--------------------------------------------------------
set targetSaveName to "test-in-orbit"
if (count of argv) > 0 then
    set targetSaveName to item 1 of argv
    log "📌 Loading save: " & targetSaveName
else
    log "📌 Using default save: " & targetSaveName
end if

--------------------------------------------------------
-- Bring KSP to foreground
--------------------------------------------------------
tell application "System Events"
    if not (exists application process "KSP") then
        log "❌ KSP is not running! Use StartKSP.scpt instead."
        return
    end if

    tell application process "KSP" to set frontmost to true
end tell

delay 2

--------------------------------------------------------
-- Clear any open menus/dialogs first
--------------------------------------------------------
log "Clearing any open menus..."
pressSpecial(53)  -- ESC key
pressSpecial(53)  -- ESC key
pressSpecial(53)  -- ESC key
pressSpecial(53)  -- ESC key
delay 1

--------------------------------------------------------
-- Pause menu -> Load Game
--------------------------------------------------------
log "Opening pause menu..."
pressSpecial(53)  -- ESC key
delay 1

log "Navigating to Load Game..."
pressSpecial(125) -- Down arrow
pressSpecial(124) -- Left arrow
pressSpecial(125)
pressSpecial(125)
pressSpecial(123) -- Right arrow
pressSpecial(36)  -- Return
delay 2

--------------------------------------------------------
-- Search for target save
--------------------------------------------------------
log "Searching for save: " & targetSaveName
pressSpecial(48)  -- Tab to search field

repeat with char in targetSaveName
    pressKey(char as text)
end repeat

pressSpecial(36)  -- Return

--------------------------------------------------------
-- Select result and load
--------------------------------------------------------
log "Loading save..."
pressSpecial(125) -- Down to first result
pressSpecial(36)  -- Select it
pressSpecial(36)  -- Load it
delay 20


--------------------------------------------------------
-- Done
--------------------------------------------------------

log "✅ Save loaded successfully!"

end main
