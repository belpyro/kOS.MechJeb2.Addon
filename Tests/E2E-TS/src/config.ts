/**
 * Configuration for E2E tests
 *
 * Loads environment variables and provides platform-specific paths.
 */

import { config } from 'dotenv';
import { join } from 'path';
import { existsSync } from 'fs';
import { homedir, tmpdir } from 'os';
import { fileURLToPath } from 'url';
import { dirname } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Load .env file from E2E-TS directory
config({ path: join(__dirname, '..', '.env') });

// Platform detection
export const platform = process.platform;
export const isMacOS = platform === 'darwin';
export const isLinux = platform === 'linux';
export const isWindows = platform === 'win32';

// Validate required configuration
function getRequiredEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(
      `ERROR: ${name} environment variable not set.\n` +
      `Create Tests/E2E-TS/.env with: ${name}=/path/to/Kerbal Space Program`
    );
  }
  return value;
}

// Core paths
export const KSP_DIR = getRequiredEnv('KSP_DIR');

// Verify KSP_DIR exists
if (!existsSync(KSP_DIR)) {
  throw new Error(`ERROR: KSP_DIR does not exist: ${KSP_DIR}`);
}

// Derived paths
export const KSP_SAVES = join(KSP_DIR, 'saves');
export const KSP_GAMEDATA = join(KSP_DIR, 'GameData');
export const KSP_LOG = join(KSP_DIR, 'KSP.log');
export const AUTOLOAD_CONFIG = join(KSP_GAMEDATA, 'KSP-AutoLoad', 'AutoLoad.cfg');

// Platform-specific paths
export const KSP_APP = process.env.KSP_APP || (() => {
  if (isMacOS) return join(KSP_DIR, 'KSP.app');
  if (isLinux) return join(KSP_DIR, 'KSP.x86_64');
  if (isWindows) return join(KSP_DIR, 'KSP_x64.exe');
  return join(KSP_DIR, 'KSP');
})();

export const PLAYER_LOG = process.env.PLAYER_LOG || (() => {
  if (isMacOS) return join(homedir(), 'Library', 'Logs', 'Squad', 'KSP', 'Player.log');
  if (isLinux) return join(homedir(), '.config', 'unity3d', 'Squad', 'Kerbal Space Program', 'Player.log');
  if (isWindows) return join(KSP_DIR, 'KSP_x64_Data', 'output_log.txt');
  return join(KSP_DIR, 'KSP_Data', 'output_log.txt');
})();

// State tracking - use os.tmpdir() for cross-platform support
export const LAST_SAVE_FILE = join(tmpdir(), 'ksp-e2e-last-save');

// kOS connection settings
export const KOS_HOST = '127.0.0.1';
export const KOS_PORT = 5410;
export const KOS_CPU_LABEL = 'guidance';

// Test timeouts (ms)
export const TIMEOUTS = {
  KSP_STARTUP: 420000,     // 7 minutes for KSP to start
  KOS_READY: 180000,       // 3 minutes for kOS to be ready
  VESSEL_INIT: 60000,      // 1 minute for vessel initialization
  MANEUVER_OPERATION: 30000, // 30 seconds for maneuver operations
  BURN_EXECUTION: 2400000,  // 40 minutes for burn execution
};

// Log patterns for watching
// Note: Use simple patterns that work for both fresh start and AutoLoad reload
export const LOG_PATTERNS = {
  FLIGHT_SCENE: /to FLIGHT/,  // Matches both "From LOADING to FLIGHT" and "From MAINMENU to FLIGHT"
  KOS_TELNET_READY: /kOS: TelnetMainServer started listening on/,
  KOS_VESSEL_READY: /kOS: OnStart:.*READY/,
  AUTOLOAD_LOADING: /KSP_AutoLoad: Trying to load/,
};

// Save configurations
export const SAVES = {
  ORBIT: 'test2',      // Ship in orbit (for maneuver tests)
  LAUNCHPAD: 'test1',  // Ship on launchpad (for ascent tests)
  DIRECTORY: 'a_test',
};

/**
 * Get full path to a save file
 */
export function getSavePath(saveName: string): string {
  return join(KSP_SAVES, SAVES.DIRECTORY, `${saveName}.sfs`);
}

/**
 * Check if a save file exists
 */
export function saveExists(saveName: string): boolean {
  return existsSync(getSavePath(saveName));
}

// Export a config object for convenience
export const config_obj = {
  KSP_DIR,
  KSP_SAVES,
  KSP_GAMEDATA,
  KSP_LOG,
  KSP_APP,
  PLAYER_LOG,
  AUTOLOAD_CONFIG,
  LAST_SAVE_FILE,
  KOS_HOST,
  KOS_PORT,
  KOS_CPU_LABEL,
  TIMEOUTS,
  LOG_PATTERNS,
  SAVES,
  platform,
  isMacOS,
  isLinux,
  isWindows,
};

export default config_obj;
