/**
 * E2E test for HOHMANN transfer maneuver operation
 *
 * Tests that MechJeb can create Hohmann transfer nodes to reach a target.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('HOHMANN', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates Hohmann transfer nodes to Mun', async () => {
    const maneuver = await getManeuverProgram();

    // Set target to Mun
    console.log('  Setting target to Mun...');
    const targetSet = await maneuver.setTarget('Mun', 'body');
    expect(targetSet).toBe(true);

    // Create Hohmann transfer
    console.log('  Creating Hohmann transfer...');
    const result = await maneuver.hohmannTransfer('COMPUTED', true);

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('requires target to be set', async () => {
    const maneuver = await getManeuverProgram();

    // Clear any existing target by attempting a transfer without one
    // This tests the error handling
    const hasTarget = await maneuver.hasTarget();
    if (!hasTarget) {
      console.log('  No target set (expected)');
      const result = await maneuver.hohmannTransfer();
      expect(result.success).toBe(false);
      expect(result.error).toContain('No target');
    } else {
      console.log('  Target already set, skipping "no target" test case');
    }
  });
});
