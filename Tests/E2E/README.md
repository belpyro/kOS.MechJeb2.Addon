# E2E Tests for kOS.MechJeb2.Addon

End-to-end TypeScript tests for the kOS MechJeb2 addon. These tests run actual
maneuvers in KSP to verify the addon works correctly.

## Prerequisites

### 1. KSP Installation

Set the `KSP_DIR` environment variable to your KSP installation path.

**Option A: Create a `.env` file (recommended)**

Create a file named `.env` in the `Tests/E2E/` directory:

```ini
# macOS
KSP_DIR=/Users/yourname/Library/Application Support/Steam/steamapps/common/Kerbal Space Program

# Linux
KSP_DIR=/home/yourname/.steam/steam/steamapps/common/Kerbal Space Program

# Windows
KSP_DIR=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program
```

**Option B: Set environment variable**

```bash
# macOS/Linux
export KSP_DIR="/path/to/Kerbal Space Program"

# Windows (PowerShell)
$env:KSP_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"

# Windows (Command Prompt)
set KSP_DIR=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program
```

### 2. Required KSP Add-ons

Install these mods via CKAN:
  ckan install kOS MechJebForAll kOSforAll KSP-AutoLoad

Right now we nee dev Version of MechJeb2:
  https://ksp.sarbian.com/jenkins/job/MechJeb2-Dev/


### 3. Node.js Setup

```bash
cd Tests/E2E
npm install
```

**Important**: The `ksp-mcp` package provides `KosConnection` for kOS telnet
communication and the kOS.MechJeb2.Addon calls.

### 4. Test Assets

The test save files are automatically copied on first run. To copy manually:

```bash
# macOS/Linux
cp -r asset/stock/ "$KSP_DIR/saves/stock/"
cp asset/AutoLoad.cfg "$KSP_DIR/GameData/KSP-AutoLoad/"

# Windows (PowerShell)
Copy-Item -Recurse asset\stock\ "$env:KSP_DIR\saves\stock\"
Copy-Item asset\AutoLoad.cfg "$env:KSP_DIR\GameData\KSP-AutoLoad\"
```

## Running Tests

```bash
# Validate environment only (quick check)
npm run validate

# Run all tests (mission order)
npm test

# Run specific test
npm run test:circularize
npm run test:hohmann
npm run test:ascent

# Watch mode
npm run test:watch
```

## Test Order

Tests run in mission order (via custom sequencer):

1. **ascent** - Launch from pad to orbit
2. **circularize** - Circularize orbit
3. **changeap/changepe** - Adjust orbit
4. **ellipticize** - Create elliptical orbit
5. **changeinclination** - Plane change
6. **lan/longitude** - Orbital element changes
7. **hohmann** - Transfer maneuver
8. **coursecorrection** - Mid-course correction
9. **killrelvel** - Match velocities
10. **resonant** - Resonant orbit

## Platform Support

| Platform | KSP Launch | Hot Reload | Player.log |
|----------|------------|------------|------------|
| macOS | Yes | AppleScript | `~/Library/Logs/Squad/KSP/Player.log` |
| Linux | Yes | Restart | `~/.config/unity3d/Squad/.../Player.log` |
| Windows | Yes | Restart | `KSP_x64_Data/output_log.txt` |

### Chained Mode (Optimal for Test Suites)

Tests run in **chained mode** by default - after the first test launches KSP and loads a save,
subsequent tests reuse the running KSP instance. This dramatically speeds up test execution:

- **First test**: Launches KSP, waits for flight scene (~3-5 min)
- **Subsequent tests**: Hot-reload save or clear nodes only (~5-30 sec)

The test framework detects if KSP is already running with the correct save and skips
unnecessary restarts. On macOS, AppleScript enables in-game save reloading without
quitting KSP. On Linux/Windows, tests restart KSP when a different save is needed.

## Troubleshooting

### "KSP_DIR not set"
Create a `.env` file in `Tests/E2E/` or set the environment variable.

### "Missing required addon: X"
Install the missing mod in `[KSP_DIR]/GameData/`.

### "ksp-mcp not installed"
Run `npm install` in the Tests/E2E directory.

### Tests timeout waiting for KSP
- Ensure KSP can start normally
- Check that kOS telnet is enabled (port 5410)
- Verify MechJeb is available on the vessel

### Save file issues
- The test uses saves in `saves/stock/` directory
- `persistent.sfs` - Vessel on launchpad (ascent tests)
- `test-in-orbit.sfs` - Vessel in orbit (maneuver tests)

## Architecture

```
Tests/E2E/
├── src/
│   ├── config.ts              # Platform-specific configuration
│   ├── validate-environment.ts # Pre-test validation
│   ├── helpers/
│   │   ├── test-setup.ts      # Jest setup and shared instances
│   │   ├── ksp-launcher.ts    # KSP process control
│   │   ├── kos-waiter.ts      # kOS connection waiting
│   │   └── log-watcher.ts     # Log file monitoring
│   └── tests/
│       └── *.test.ts          # Individual test files
├── asset/
│   ├── stock/                 # Test save files
│   └── AutoLoad.cfg           # KSP-AutoLoad config
└── jest.config.js             # Jest configuration
```

## Building kOS.MechJeb2.Addon

The addon must be built and deployed to your KSP GameData folder:

```bash
# From the repository root (not Tests/E2E)
cd /path/to/kOS.MechJeb2.Addon

# macOS/Linux
./build.sh Debug    # or Release

# Windows (PowerShell) - requires .NET SDK
dotnet build kOS.MechJeb2.Addon/kOS.MechJeb2.Addon.csproj -c Debug
```

The build script auto-deploys to KSP GameData via KSPBuildTools. After building:
- Restart KSP to load the new DLL
- Verify with: `PRINT ADDONS:MJ.` in kOS terminal

## Contributing

When adding new tests:
1. Follow the existing test pattern in `src/tests/`
2. Use `ensureKspReady(SAVES.ORBIT)` or `ensureKspReady(SAVES.LAUNCHPAD)`
3. Call `clearNodes()` in `beforeEach` to ensure clean state
4. Add the test to `mission-sequencer.cjs` if order matters
