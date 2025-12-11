/**
 * E2E test for ELLIPTICIZE maneuver operation
 *
 * Tests that MechJeb can create a maneuver node to reshape orbit to specific Pe/Ap.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('ELLIPTICIZE', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates node to ellipticize orbit to 80km x 120km', async () => {
    const maneuver = await getManeuverProgram();
    const targetPe = 80000;  // 80km
    const targetAp = 120000; // 120km

    console.log(`  Creating node to reshape orbit to ${targetPe / 1000}km x ${targetAp / 1000}km...`);
    const result = await maneuver.ellipticize(targetPe, targetAp, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });
});
