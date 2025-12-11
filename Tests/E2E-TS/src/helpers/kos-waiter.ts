/**
 * kOS connection waiting utilities
 *
 * Wait for kOS telnet server to be ready and accessible.
 */

import { createConnection, Socket } from 'net';
import { KOS_HOST, KOS_PORT, TIMEOUTS } from '../config.js';

/**
 * Check if kOS telnet is ready by validating "Choose a CPU" response
 *
 * This matches the bash implementation: echo "" | nc -w 1 127.0.0.1 5410 | grep -q "Choose a CPU"
 */
export async function isKosReady(): Promise<boolean> {
  return new Promise((resolve) => {
    const socket = new Socket();
    let data = '';

    const timeout = setTimeout(() => {
      socket.destroy();
      resolve(false);
    }, 2000);

    socket.once('connect', () => {
      // Send empty line like bash does
      socket.write('\n');
    });

    socket.on('data', (chunk) => {
      data += chunk.toString();
      // Check for "Choose a CPU" which indicates kOS is ready
      if (data.includes('Choose a CPU')) {
        clearTimeout(timeout);
        socket.destroy();
        resolve(true);
      }
    });

    socket.once('error', () => {
      clearTimeout(timeout);
      resolve(false);
    });

    socket.once('close', () => {
      clearTimeout(timeout);
      // Check data one more time on close
      resolve(data.includes('Choose a CPU'));
    });

    socket.connect(KOS_PORT, KOS_HOST);
  });
}

/**
 * Check if kOS telnet port is accessible (basic port check)
 */
export async function isKosPortOpen(): Promise<boolean> {
  return new Promise((resolve) => {
    const socket = new Socket();
    const timeout = setTimeout(() => {
      socket.destroy();
      resolve(false);
    }, 1000);

    socket.once('connect', () => {
      clearTimeout(timeout);
      socket.destroy();
      resolve(true);
    });

    socket.once('error', () => {
      clearTimeout(timeout);
      resolve(false);
    });

    socket.connect(KOS_PORT, KOS_HOST);
  });
}

/**
 * Wait for kOS telnet server to be accessible and ready
 *
 * Uses isKosReady() to validate "Choose a CPU" response, matching bash behavior.
 *
 * @param timeoutMs Timeout in milliseconds
 * @param pollIntervalMs Polling interval
 */
export async function waitForKos(
  timeoutMs: number = TIMEOUTS.KOS_READY,
  pollIntervalMs: number = 1000
): Promise<void> {
  console.log(`  Waiting for kOS telnet on ${KOS_HOST}:${KOS_PORT}...`);

  const startTime = Date.now();
  let elapsed = 0;

  // Fast path: check if already ready
  if (await isKosReady()) {
    console.log('  kOS telnet is ready');
    return;
  }

  while (Date.now() - startTime < timeoutMs) {
    await delay(pollIntervalMs);
    elapsed = Math.floor((Date.now() - startTime) / 1000);

    if (await isKosReady()) {
      console.log(`  kOS telnet is ready (${elapsed}s)`);
      return;
    }

    // Status update every 15 seconds
    if (elapsed % 15 === 0 && elapsed > 0) {
      console.log(`  Still waiting... (${elapsed}s elapsed)`);
    }
  }

  throw new Error(`Timeout waiting for kOS telnet on ${KOS_HOST}:${KOS_PORT}`);
}

/**
 * Wait for kOS connection to be fully ready
 *
 * Combines port check with a test command to ensure kOS is responsive.
 */
export async function waitForKosReady(
  timeoutMs: number = TIMEOUTS.KOS_READY
): Promise<void> {
  // First wait for port to be accessible
  await waitForKos(timeoutMs);

  // Give kOS a moment to be fully ready
  await delay(2000);

  console.log('  kOS is ready');
}

/**
 * Simple delay helper
 */
function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export { delay };
