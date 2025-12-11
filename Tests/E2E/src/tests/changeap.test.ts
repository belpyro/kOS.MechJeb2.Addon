/**
 * E2E test for CHANGEAP maneuver operation
 *
 * Tests that MechJeb can create a maneuver node to change apoapsis.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('CHANGEAP', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates node to raise apoapsis to 150km', async () => {
    const maneuver = await getManeuverProgram();
    const targetAp = 150000; // 150km

    console.log(`  Creating node to raise apoapsis to ${targetAp / 1000}km...`);
    const result = await maneuver.adjustApoapsis(targetAp, 'PERIAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates node to lower apoapsis to 90km', async () => {
    const maneuver = await getManeuverProgram();
    const targetAp = 90000; // 90km

    console.log(`  Creating node to lower apoapsis to ${targetAp / 1000}km...`);
    const result = await maneuver.adjustApoapsis(targetAp, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    // Lowering apoapsis still requires fuel
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });
});
