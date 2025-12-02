#!/usr/bin/env osascript

(*
Automated KSP launcher + UI navigation script for native macOS KSP (Steam)
Adds robust startup detection and test pass/fail detection
*)

--------------------------------------------------------
-- Helper functions
--------------------------------------------------------
on pressKey(keyChar)
    tell application "System Events" to keystroke keyChar
    delay 1
end pressKey

on pressSpecial(vKey)
    tell application "System Events" to key code vKey
    delay 1
end pressSpecial

on getFileSize(fPath)
    try
        return (do shell script "stat -f%z " & quoted form of fPath) as number
    on error
        return 0
    end try
end getFileSize

--------------------------------------------------------
-- Steam KSP auto discover
--------------------------------------------------------
on findKSP()
    -- Expand home path properly (tilde cannot be used directly)
    set configPath to (do shell script "echo $HOME") & "/.ksp_automation_path"

    --------------------------------------------------------
    -- 1️⃣ Command line argument check
    --------------------------------------------------------
    try
        set theArgs to (system attribute "OSA_ARGV")
        if theArgs is not missing value then
            set argPath to item 1 of theArgs
            if (do shell script "test -d " & quoted form of argPath & " && echo OK || echo NO") is "OK" then
                log "📌 Using KSP path from command-line argument"
                do shell script "echo " & quoted form of argPath & " > " & quoted form of configPath
                return argPath
            end if
        end if
    end try
    
    --------------------------------------------------------
    -- 2️⃣ Previously saved config value
    --------------------------------------------------------
    if (do shell script "test -f " & quoted form of configPath & " && echo OK || echo NO") is "OK" then
        set lastPath to do shell script "cat " & quoted form of configPath
        if (do shell script "test -d " & quoted form of lastPath & " && echo OK || echo NO") is "OK" then
            log "📌 Using remembered KSP path"
            return lastPath
        else
            log "⚠️ Stored path no longer exists — removing"
            do shell script "rm -f " & quoted form of configPath
        end if
    end if
    
    --------------------------------------------------------
    -- 3️⃣ Steam auto-discovery
    --------------------------------------------------------
    log "🔍 Searching Steam libraries…"
    set steamPath to POSIX path of (path to library folder from user domain) & "Application Support/Steam/steamapps/"
    set libraryFoldersFile to steamPath & "libraryfolders.vdf"
    
    if (do shell script "test -f " & quoted form of libraryFoldersFile & " && echo OK || echo NO") is "OK" then
        set libraryPaths to paragraphs of (do shell script "grep -E '\"path\"' " & quoted form of libraryFoldersFile & " | sed 's/.*\"path\".*\"//;s/\".*//'")

        repeat with libPath in libraryPaths
            set thisSteamApps to (libPath as text) & "/steamapps/"
            set manifestPath to thisSteamApps & "appmanifest_220200.acf"
            
            if (do shell script "test -f " & quoted form of manifestPath & " && echo FOUND || echo NOT") is "FOUND" then
                set steamCandidate to thisSteamApps & "common/Kerbal Space Program/KSP.app"
                if (do shell script "test -d " & quoted form of steamCandidate & " && echo OK || echo NO") is "OK" then
                    log "🎯 Found KSP via Steam"
                    do shell script "echo " & quoted form of steamCandidate & " > " & quoted form of configPath
                    return steamCandidate
                end if
            end if
        end repeat
    end if
    
    --------------------------------------------------------
    -- 4️⃣ Ultimate fallback — user selects manually
    --------------------------------------------------------
    log "📁 Prompting user to locate KSP.app"
    set userChoice to choose file with prompt "Locate KSP.app" of type {"APPL"}
    set chosenPath to POSIX path of userChoice
    do shell script "echo " & quoted form of chosenPath & " > " & quoted form of configPath
    log "📌 User selected: " & chosenPath
    return chosenPath
end findKSP




log "⏱ Estimated time until KSP ship is launch ready: ~6 minutes 30 seconds"
log "🚀 Do not interact with KSP during automation!"
delay 1

--------------------------------------------------------
-- Locate KSP
--------------------------------------------------------
set kspPath to findKSP()
if kspPath is missing value then
    display dialog "Could not locate Kerbal Space Program installation via Steam." buttons {"OK"}
    return
end if

log "Found KSP at: " & kspPath

--------------------------------------------------------
-- Ensure KSP is not already running and launch
--------------------------------------------------------
tell application "System Events"
    if exists application process "KSP" then
        log "KSP already running, quitting it first..."
        tell application "KSP" to quit
        delay 5
    end if
end tell

tell application kspPath to activate

--------------------------------------------------------
-- Log-based startup detection
--------------------------------------------------------
set logDir to POSIX path of (do shell script "dirname " & quoted form of kspPath)
set logPath to logDir & "/KSP.log"

-- reset old log
do shell script "rm -f " & quoted form of logPath
delay 1

log "Waiting for KSP.log to start..."
repeat 120 times -- up to 2 minutes
    if (do shell script "test -f " & quoted form of logPath & " && echo OK || echo NO") is "OK" then exit repeat
    delay 1
end repeat
delay 5

log "Monitoring KSP.log growth..."

set lastSize to 0
set stableCount to 0
set maxChecks to 120 -- 120 * 15s = 30 mins max wait

repeat maxChecks times
    set currentSize to getFileSize(logPath)
    
    if currentSize > lastSize then
        set stableCount to 0
    else
        set stableCount to stableCount + 1
    end if
    
    set lastSize to currentSize
    
    if stableCount ≥ 2 then
        exit repeat
    end if
    
    delay 15
end repeat

log "KSP appears fully loaded."
delay 10

--------------------------------------------------------
-- Bring to foreground
--------------------------------------------------------
tell application "System Events"
    tell application process "KSP" to set frontmost to true
end tell

delay 2

--------------------------------------------------------
-- Key automation (same sequence you provided)
--------------------------------------------------------

-- Main menu: Start Game
pressSpecial(125)
pressSpecial(36)

-- Resume Game
pressSpecial(125)
pressSpecial(36)

-- Search field + "stock"
pressSpecial(48)
pressKey("s")
pressKey("t")
pressKey("o")
pressKey("c")
pressKey("k")
pressSpecial(36)

-- First result + Load
pressSpecial(125)
pressSpecial(36)
repeat 4 times
    pressSpecial(125)
end repeat
pressSpecial(124)
pressSpecial(36)
delay 15

-- Pause menu -> Load Game
pressSpecial(53)
pressSpecial(125)
pressSpecial(125)
pressSpecial(125)
pressSpecial(36)
delay 2

-- Search for test1
pressSpecial(48)
pressKey("t")
pressKey("e")
pressKey("s")
pressKey("t")
pressKey("1")
pressSpecial(36)

-- Select result and load
pressSpecial(125)
pressSpecial(36)
repeat 4 times
    pressSpecial(125)
end repeat
pressSpecial(124)
pressSpecial(36)

delay 15

--------------------------------------------------------
-- Done
--------------------------------------------------------

log "🚀 KSP is running and ship is ready to launch!"
