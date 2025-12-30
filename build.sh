#!/bin/bash
# Build script for kOS.MechJeb2.Addon
# Usage: ./build.sh [Debug|Release]

set -e

CONFIG="${1:-Debug}"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$PROJECT_DIR"

echo "==========================================="
echo "Building kOS.MechJeb2.Addon ($CONFIG)"
echo "==========================================="

# Clean previous build
echo "Cleaning previous build..."
rm -rf kOS.MechJeb2.Addon/bin kOS.MechJeb2.Addon/obj

# Build
echo "Building..."
/opt/homebrew/opt/dotnet@8/bin/dotnet build \
    kOS.MechJeb2.Addon/kOS.MechJeb2.Addon.csproj \
    -c "$CONFIG"

# Verify DLL was created
DLL_PATH="kOS.MechJeb2.Addon/bin/$CONFIG/kOS.MechJeb2.Addon.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "❌ Build failed - DLL not found at $DLL_PATH"
    exit 1
fi

echo ""
echo "✅ Build successful!"
ls -lh "$DLL_PATH"
shasum -a 256 "$DLL_PATH"

# Show GameData deployment status
GAMEDATA_DLL="/Volumes/Flatty/SteamLibrary/steamapps/common/Kerbal Space Program/GameData/kOS.MechJeb2.Addon/kOS.MechJeb2.Addon.dll"
if [ -f "$GAMEDATA_DLL" ]; then
    echo ""
    echo "GameData DLL:"
    ls -lh "$GAMEDATA_DLL"
    shasum -a 256 "$GAMEDATA_DLL"
else
    echo ""
    echo "⚠️  No DLL found in GameData"
fi

echo ""
echo "==========================================="
echo "⚠️  IMPORTANT: KSP must be restarted to load the new DLL!"
echo "==========================================="
echo ""
echo "To deploy and test:"
echo "  1. pkill -9 KSP"
echo "  2. Run your test script (it will start KSP automatically)"
echo ""
