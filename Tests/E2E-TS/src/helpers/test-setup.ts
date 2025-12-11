/**
 * Jest test setup
 *
 * Provides shared kOS connection and MechJeb program instances for tests.
 */

import { KosConnection } from 'ksp-mcp/transport';
import { ManeuverProgram, AscentProgram } from 'ksp-mcp/mechjeb';
import { KOS_CPU_LABEL, TIMEOUTS, SAVES } from '../config.js';
import { initializeKsp } from './ksp-launcher.js';
import { waitForKosReady } from './kos-waiter.js';

// Shared instances
let conn: KosConnection | null = null;
let maneuver: ManeuverProgram | null = null;
let ascent: AscentProgram | null = null;

// Test state
let currentSave: string | null = null;

/**
 * Get or create the shared kOS connection
 */
export async function getConnection(): Promise<KosConnection> {
  if (!conn || !conn.isConnected()) {
    conn = new KosConnection({ cpuLabel: KOS_CPU_LABEL });
    await conn.connect();
  }
  return conn;
}

/**
 * Get the maneuver program (creates if needed)
 */
export async function getManeuverProgram(): Promise<ManeuverProgram> {
  if (!maneuver) {
    const connection = await getConnection();
    maneuver = new ManeuverProgram(connection);
  }
  return maneuver;
}

/**
 * Get the ascent program (creates if needed)
 */
export async function getAscentProgram(): Promise<AscentProgram> {
  if (!ascent) {
    const connection = await getConnection();
    ascent = new AscentProgram(connection);
  }
  return ascent;
}

/**
 * Ensure KSP is ready with the correct save
 *
 * @param saveName Save file to load
 * @param options.forceRestart Always kill KSP and do fresh launch (required for ASCENT - vessel must be on pad)
 * @param options.forceReload Skip same-save optimization but allow hot reload (for tests that change vessel state)
 */
export async function ensureKspReady(
  saveName: string,
  options: { forceRestart?: boolean; forceReload?: boolean } = {}
): Promise<void> {
  // Initialize KSP (handles save switching and same-save optimization)
  await initializeKsp(saveName, options);
  currentSave = saveName;

  // Reconnect if needed
  if (conn && !conn.isConnected()) {
    conn = null;
    maneuver = null;
    ascent = null;
  }

  // Ensure connection is ready
  await getConnection();
}

/**
 * Clear ALL existing maneuver nodes
 * Mirrors bash helper: FOR N IN ALLNODES { REMOVE N. }
 */
export async function clearNodes(): Promise<void> {
  const connection = await getConnection();
  // Remove all nodes, not just NEXTNODE (Hohmann with capture creates multiple nodes)
  await connection.execute('FOR N IN ALLNODES { REMOVE N. }', 5000).catch(() => {});
}

// Jest global setup
beforeAll(async () => {
  console.log('\n========================================');
  console.log('E2E Test Suite Starting');
  console.log('========================================\n');
}, TIMEOUTS.KSP_STARTUP);

// Jest global teardown
afterAll(async () => {
  console.log('\n========================================');
  console.log('E2E Test Suite Complete');
  console.log('========================================\n');

  // Disconnect
  if (conn) {
    await conn.disconnect();
    conn = null;
  }
});

// Export for test files
export {
  conn,
  maneuver,
  ascent,
  currentSave,
  SAVES,
  TIMEOUTS,
};
