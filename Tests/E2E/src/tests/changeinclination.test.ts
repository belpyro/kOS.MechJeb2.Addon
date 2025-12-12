/**
 * E2E test for CHANGEINCLINATION maneuver operation
 *
 * Tests that MechJeb can create a maneuver node to change orbital inclination.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('CHANGEINCLINATION', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates node to change inclination to 0 degrees (equatorial)', async () => {
    const maneuver = await getManeuverProgram();
    const targetInc = 0; // Equatorial orbit

    console.log(`  Creating node to change inclination to ${targetInc} degrees...`);
    const result = await maneuver.changeInclination(targetInc, 'EQ_NEAREST_AD');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    // May have small or zero dV if already close to equatorial
    expect(result.deltaV).toBeGreaterThanOrEqual(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates node to change inclination to 10 degrees', async () => {
    const maneuver = await getManeuverProgram();
    const targetInc = 10;

    console.log(`  Creating node to change inclination to ${targetInc} degrees...`);
    const result = await maneuver.changeInclination(targetInc, 'EQ_NEAREST_AD');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });
});
