/**
 * Log file watcher utilities
 *
 * Watch log files for specific patterns and wait for events.
 */

import { createReadStream, statSync, existsSync, readFileSync } from 'fs';
import { createInterface } from 'readline';
import { PLAYER_LOG, LOG_PATTERNS } from '../config.js';

/**
 * Get current line count of log file
 *
 * Used to track log position before reload so we only check NEW patterns.
 */
export function getLogLineCount(logPath: string = PLAYER_LOG): number {
  if (!existsSync(logPath)) {
    return 0;
  }
  try {
    const content = readFileSync(logPath, 'utf-8');
    return content.split('\n').length;
  } catch {
    return 0;
  }
}

/**
 * Watch a log file for a pattern
 *
 * @param logPath Path to log file
 * @param pattern Pattern to match (string or RegExp)
 * @param timeoutMs Timeout in milliseconds
 * @returns Promise that resolves with matching line or rejects on timeout
 */
export async function watchLog(
  logPath: string,
  pattern: string | RegExp,
  timeoutMs: number
): Promise<string> {
  const regex = typeof pattern === 'string' ? new RegExp(pattern) : pattern;

  return new Promise((resolve, reject) => {
    const timeout = setTimeout(() => {
      reject(new Error(`Timeout waiting for pattern: ${pattern}`));
    }, timeoutMs);

    // Check if file exists
    if (!existsSync(logPath)) {
      clearTimeout(timeout);
      reject(new Error(`Log file does not exist: ${logPath}`));
      return;
    }

    // Get initial file size
    const stats = statSync(logPath);
    let lastSize = stats.size;
    let readPosition = stats.size; // Start from end of file

    const checkNewLines = async () => {
      try {
        const currentStats = statSync(logPath);
        if (currentStats.size > readPosition) {
          // Read new content
          const stream = createReadStream(logPath, {
            start: readPosition,
            encoding: 'utf-8',
          });

          const rl = createInterface({ input: stream });

          for await (const line of rl) {
            if (regex.test(line)) {
              clearTimeout(timeout);
              clearInterval(interval);
              resolve(line);
              stream.destroy();
              return;
            }
          }

          readPosition = currentStats.size;
        }
      } catch (err) {
        // File may be temporarily unavailable, continue
      }
    };

    // Poll for new content
    const interval = setInterval(checkNewLines, 500);

    // Initial check
    checkNewLines();
  });
}

/**
 * Get recent lines from a log file
 *
 * @param logPath Path to log file
 * @param count Number of lines to read
 * @returns Array of lines
 */
export async function getRecentLines(logPath: string, count: number): Promise<string[]> {
  if (!existsSync(logPath)) {
    return [];
  }

  const lines: string[] = [];
  const stream = createReadStream(logPath, { encoding: 'utf-8' });
  const rl = createInterface({ input: stream });

  for await (const line of rl) {
    lines.push(line);
    if (lines.length > count) {
      lines.shift();
    }
  }

  return lines;
}

/**
 * Check recent logs for a pattern (fast path)
 *
 * @param logPath Path to log file
 * @param pattern Pattern to match
 * @param lineCount Number of recent lines to check
 * @returns Matching line or null
 */
export async function checkRecentLogs(
  logPath: string,
  pattern: string | RegExp,
  lineCount: number = 500
): Promise<string | null> {
  const regex = typeof pattern === 'string' ? new RegExp(pattern) : pattern;
  const lines = await getRecentLines(logPath, lineCount);

  for (const line of lines.reverse()) {
    if (regex.test(line)) {
      return line;
    }
  }

  return null;
}

/**
 * Wait for KSP to reach flight scene
 */
export async function waitForFlightScene(timeoutMs: number): Promise<void> {
  console.log('  Waiting for KSP flight scene...');

  // Fast path: check recent 500 lines first
  const recent = await checkRecentLogs(PLAYER_LOG, LOG_PATTERNS.FLIGHT_SCENE);
  if (recent) {
    console.log('  Flight scene already loaded');
    return;
  }

  await watchLog(PLAYER_LOG, LOG_PATTERNS.FLIGHT_SCENE, timeoutMs);
  console.log('  Flight scene loaded');
}

/**
 * Wait for kOS telnet server to be ready
 */
export async function waitForKosTelnet(timeoutMs: number): Promise<void> {
  console.log('  Waiting for kOS telnet server...');

  // Fast path: check recent logs first
  const recent = await checkRecentLogs(PLAYER_LOG, LOG_PATTERNS.KOS_TELNET_READY);
  if (recent) {
    console.log('  kOS telnet already ready');
    return;
  }

  await watchLog(PLAYER_LOG, LOG_PATTERNS.KOS_TELNET_READY, timeoutMs);
  console.log('  kOS telnet ready');
}

/**
 * Check for pattern in log lines after a specific start line
 */
export async function checkLogAfterLine(
  logPath: string,
  pattern: string | RegExp,
  startLine: number
): Promise<boolean> {
  if (!existsSync(logPath)) {
    return false;
  }

  try {
    const content = readFileSync(logPath, 'utf-8');
    const lines = content.split('\n');
    const regex = typeof pattern === 'string' ? new RegExp(pattern) : pattern;

    // Check lines after startLine (1-indexed)
    for (let i = startLine; i < lines.length; i++) {
      if (regex.test(lines[i])) {
        return true;
      }
    }
  } catch {
    // File may be temporarily unavailable
  }
  return false;
}

/**
 * Wait for vessel to be initialized in kOS
 *
 * @param timeoutMs Timeout in milliseconds
 * @param startLine If provided, only check for pattern AFTER this line (used after reload)
 */
export async function waitForVesselInit(
  timeoutMs: number,
  startLine: number = 0
): Promise<void> {
  console.log('  Waiting for kOS vessel initialization...');

  const pollIntervalMs = 2000;
  const startTime = Date.now();
  let elapsed = 0;

  // If no start line, check recent logs first (fast path for fresh starts)
  if (startLine === 0) {
    const recent = await checkRecentLogs(PLAYER_LOG, LOG_PATTERNS.KOS_VESSEL_READY);
    if (recent) {
      console.log('  Vessel already initialized');
      return;
    }
  }

  // Poll for pattern (after startLine if specified)
  while (Date.now() - startTime < timeoutMs) {
    if (startLine > 0) {
      // Only check lines after startLine
      if (await checkLogAfterLine(PLAYER_LOG, LOG_PATTERNS.KOS_VESSEL_READY, startLine)) {
        elapsed = Math.floor((Date.now() - startTime) / 1000);
        console.log(`  Vessel initialized (${elapsed}s)`);
        return;
      }
    } else {
      // Check recent logs
      const recent = await checkRecentLogs(PLAYER_LOG, LOG_PATTERNS.KOS_VESSEL_READY);
      if (recent) {
        elapsed = Math.floor((Date.now() - startTime) / 1000);
        console.log(`  Vessel initialized (${elapsed}s)`);
        return;
      }
    }

    await new Promise((r) => setTimeout(r, pollIntervalMs));
    elapsed = Math.floor((Date.now() - startTime) / 1000);

    // Status update every 15 seconds
    if (elapsed % 15 === 0 && elapsed > 0) {
      console.log(`  Still waiting... (${elapsed}s elapsed)`);
    }
  }

  throw new Error(`Timeout waiting for vessel initialization (${timeoutMs}ms)`);
}
