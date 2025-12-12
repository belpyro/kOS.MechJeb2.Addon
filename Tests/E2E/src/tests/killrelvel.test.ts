/**
 * E2E test for KILLRELVEL maneuver operation
 *
 * Tests that MechJeb can create a node to match velocity with target.
 * Most useful for rendezvous operations.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES, TIMEOUTS } from '../helpers/test-setup.js';

describe('KILLRELVEL', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  }, TIMEOUTS.KSP_STARTUP);

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates kill relative velocity node at closest approach', async () => {
    const maneuver = await getManeuverProgram();

    // Set a target (Mun works for testing - will show large velocity difference)
    console.log('  Setting target to Mun...');
    await maneuver.setTarget('Mun');

    console.log('  Creating kill relative velocity node at closest approach...');
    const result = await maneuver.killRelVel('CLOSEST_APPROACH');

    // This will create a node to match Mun's velocity at closest approach
    // The deltaV will be large since we're matching a moon's orbital velocity
    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  }, TIMEOUTS.BURN_EXECUTION);

  it('creates kill relative velocity node with X_FROM_NOW timing', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Setting target to Mun...');
    await maneuver.setTarget('Mun');

    // Note: KILLRELVEL only supports CLOSEST_APPROACH and X_FROM_NOW
    // PERIAPSIS is not a valid timeRef for this operation
    console.log('  Creating kill relative velocity node (60s from now)...');
    const result = await maneuver.killRelVel('X_FROM_NOW');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
  });
});
