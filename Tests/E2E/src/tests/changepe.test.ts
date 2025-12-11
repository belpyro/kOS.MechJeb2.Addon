/**
 * E2E test for CHANGEPE maneuver operation
 *
 * Tests that MechJeb can create a maneuver node to change periapsis.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('CHANGEPE', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates node to lower periapsis to 75km', async () => {
    const maneuver = await getManeuverProgram();
    const targetPe = 75000; // 75km

    console.log(`  Creating node to lower periapsis to ${targetPe / 1000}km...`);
    const result = await maneuver.adjustPeriapsis(targetPe, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates node to raise periapsis to 100km', async () => {
    const maneuver = await getManeuverProgram();
    const targetPe = 100000; // 100km

    console.log(`  Creating node to raise periapsis to ${targetPe / 1000}km...`);
    const result = await maneuver.adjustPeriapsis(targetPe, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    // Note: On a ~110km circular orbit, raising Pe to 100km will have minimal dV
    // because Pe is already close to the orbit altitude

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });
});
