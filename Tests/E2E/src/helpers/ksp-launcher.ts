/**
 * KSP launcher utilities
 *
 * Cross-platform KSP process control and save loading.
 */

import { spawn, execSync, exec } from 'child_process';
import { writeFileSync, existsSync, unlinkSync, readFileSync, mkdirSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';
import {
  KSP_APP,
  KSP_SAVES,
  AUTOLOAD_CONFIG,
  PLAYER_LOG,
  LAST_SAVE_FILE,
  SAVES,
  TIMEOUTS,
  isMacOS,
  isLinux,
  isWindows,
  saveExists,
} from '../config.js';
import { waitForFlightScene, getLogLineCount } from './log-watcher.js';
import { waitForKos, delay } from './kos-waiter.js';

/**
 * Check if KSP is currently running
 */
export function isKspRunning(): boolean {
  try {
    if (isWindows) {
      const result = execSync('tasklist /FI "IMAGENAME eq KSP_x64.exe" 2>nul', {
        encoding: 'utf-8',
      });
      return result.includes('KSP_x64.exe');
    } else {
      const result = execSync('pgrep -q KSP', { encoding: 'utf-8' });
      return true;
    }
  } catch {
    return false;
  }
}

/**
 * Kill KSP process
 */
export async function killKsp(): Promise<void> {
  if (!isKspRunning()) {
    return;
  }

  console.log('  Killing KSP...');

  if (isMacOS) {
    // Try graceful quit first
    try {
      execSync('osascript -e \'tell application "KSP" to quit\' 2>/dev/null', {
        timeout: 5000,
      });
      await delay(5000);
    } catch {
      // Graceful quit failed, continue to force kill
    }
  }

  // Force kill
  try {
    if (isWindows) {
      execSync('taskkill /F /IM KSP_x64.exe 2>nul');
    } else {
      execSync('pkill -9 KSP');
    }
    await delay(2000);
  } catch {
    // Process may already be dead
  }
}

/**
 * Write AutoLoad configuration
 */
export function writeAutoloadConfig(saveName: string): void {
  const configDir = dirname(AUTOLOAD_CONFIG);

  // Ensure directory exists
  if (!existsSync(configDir)) {
    mkdirSync(configDir, { recursive: true });
  }

  // Write config (flat ConfigNode format)
  const config = `directory = ${SAVES.DIRECTORY}\nsavegame = ${saveName}\n`;
  writeFileSync(AUTOLOAD_CONFIG, config);
  console.log(`  AutoLoad configured for: ${SAVES.DIRECTORY}/${saveName}`);
}

/**
 * Clear Player.log for fresh monitoring
 */
export function clearPlayerLog(): void {
  try {
    if (existsSync(PLAYER_LOG)) {
      writeFileSync(PLAYER_LOG, '');
    }
  } catch {
    // May not have permission, that's okay
  }
}

/**
 * Record the last loaded save (for save reuse optimization)
 */
export function recordLastSave(saveName: string): void {
  writeFileSync(LAST_SAVE_FILE, saveName);
}

/**
 * Get the last loaded save
 */
export function getLastSave(): string | null {
  try {
    if (existsSync(LAST_SAVE_FILE)) {
      return readFileSync(LAST_SAVE_FILE, 'utf-8').trim();
    }
  } catch {
    // File doesn't exist or not readable
  }
  return null;
}

/**
 * Clear all maneuver nodes via kOS
 *
 * Uses the ksp-mcp daemon to execute: FOR N IN ALLNODES { REMOVE N. }
 */
export async function clearNodes(): Promise<boolean> {
  try {
    // Import the daemon client dynamically to avoid circular deps
    const { execute } = await import('ksp-mcp/daemon');
    const result = await execute('FOR N IN ALLNODES { REMOVE N. }');
    if (result.success) {
      console.log('  Nodes cleared');
      return true;
    }
    console.log(`  Failed to clear nodes: ${result.error}`);
    return false;
  } catch (err) {
    console.log(`  Failed to clear nodes: ${err instanceof Error ? err.message : String(err)}`);
    return false;
  }
}

/**
 * Launch KSP with AutoLoad
 *
 * @param saveName Save file to load
 * @param waitForReady Wait for flight scene and kOS
 */
export async function launchKsp(
  saveName: string,
  waitForReady: boolean = true
): Promise<void> {
  console.log(`\nLaunching KSP with save: ${saveName}`);

  // Verify save exists
  if (!saveExists(saveName)) {
    throw new Error(`Save file not found: ${saveName}`);
  }

  // Kill existing KSP
  await killKsp();

  // Configure AutoLoad
  writeAutoloadConfig(saveName);

  // Clear logs for fresh monitoring
  clearPlayerLog();

  // Launch KSP
  console.log('  Starting KSP...');
  if (isMacOS) {
    exec(`open "${KSP_APP}"`);
  } else if (isWindows) {
    spawn(KSP_APP, [], { detached: true, stdio: 'ignore' }).unref();
  } else {
    spawn(KSP_APP, [], { detached: true, stdio: 'ignore' }).unref();
  }

  // Record loaded save
  recordLastSave(saveName);

  if (waitForReady) {
    await waitForKspReady();
  }
}

/**
 * Reload save in running KSP (hot reload on macOS)
 *
 * @param saveName Save file to load
 */
export async function reloadSave(saveName: string): Promise<void> {
  console.log(`\nReloading save: ${saveName}`);

  // Verify save exists
  if (!saveExists(saveName)) {
    throw new Error(`Save file not found: ${saveName}`);
  }

  if (isMacOS && isKspRunning()) {
    // macOS: Use LoadSaveKSP.scpt AppleScript for hot reload
    console.log('  Using LoadSaveKSP.scpt hot reload...');

    // Record current log position BEFORE reload (to detect NEW initialization)
    const logLinesBefore = getLogLineCount();
    const startAfter = logLinesBefore + 1;

    // Path to the AppleScript in E2E/asset directory
    const __filename = fileURLToPath(import.meta.url);
    const scriptPath = join(dirname(dirname(dirname(__filename))), 'asset', 'LoadSaveKSP.scpt');

    try {
      // Run the AppleScript with the save name as argument
      // The script handles: ESC to clear menus, pause menu, load game, search, select, load
      execSync(`"${scriptPath}" ${saveName}`, { timeout: 60000 });
      recordLastSave(saveName);

      // Wait for kOS telnet to respond with "Choose a CPU" (validates vessel is ready)
      console.log('  Waiting for kOS to be ready...');
      await waitForKos(TIMEOUTS.VESSEL_INIT);
    } catch (err) {
      console.log('  Hot reload failed, falling back to restart...');
      await launchKsp(saveName);
    }
  } else {
    // Other platforms: restart KSP
    await launchKsp(saveName);
  }
}

/**
 * Check if kOS telnet is ready (quick TCP check)
 */
async function isKosReady(): Promise<boolean> {
  try {
    await waitForKos(5000); // 5 second quick check
    return true;
  } catch {
    return false;
  }
}

/**
 * Initialize KSP for tests
 *
 * Optimizes for test speed:
 * - KSP running + same save + kOS responding → clear nodes only (fast path)
 * - KSP running + different save + macOS → AppleScript reload
 * - KSP running + different save + non-macOS → kill + restart KSP
 * - KSP not running → fresh start
 *
 * @param saveName Save file to load
 * @param options Options for initialization
 * @param options.forceRestart Always restart KSP (kill + fresh start)
 * @param options.forceReload Skip same-save optimization, always reload
 */
export async function initializeKsp(
  saveName: string,
  options: { forceRestart?: boolean; forceReload?: boolean } = {}
): Promise<void> {
  const { forceRestart = false, forceReload = false } = options;
  const kspRunning = isKspRunning();
  const lastSave = getLastSave();

  console.log(`\nInitializing KSP for test`);
  console.log(`  Required save: ${saveName}`);
  console.log(`  KSP running: ${kspRunning}`);
  console.log(`  Last save: ${lastSave || '(none)'}`);

  if (forceRestart || !kspRunning) {
    // Fresh start (kill if running, then launch)
    await launchKsp(saveName);
    return;
  }

  // KSP is running - check for same-save fast path
  if (saveName === lastSave && !forceReload) {
    console.log(`  Same save '${saveName}' - checking if kOS is ready...`);

    // Check if kOS is responding
    if (await isKosReady()) {
      console.log('  kOS is ready - clearing nodes only (fast path)');

      // Clear nodes - fall back to reload if it fails
      const cleared = await clearNodes();
      if (!cleared) {
        console.log('  Failed to clear nodes, falling back to reload...');
        await reloadSave(saveName);
      }
      return;
    }

    console.log('  kOS not responding, falling back to reload...');
  }

  // Different save or kOS not responding - need to reload
  await reloadSave(saveName);
}

/**
 * Wait for KSP to be fully ready
 *
 * 1. Wait for flight scene (log watching)
 * 2. Wait for kOS telnet ready (TCP port check with "Choose a CPU" validation)
 */
async function waitForKspReady(): Promise<void> {
  console.log('  Waiting for KSP to be ready...');

  // Wait for flight scene (log-based)
  await waitForFlightScene(TIMEOUTS.KSP_STARTUP);

  // Wait for kOS telnet via TCP (validates "Choose a CPU" response)
  await waitForKos(TIMEOUTS.KOS_READY);

  console.log('  KSP is ready!');
}
