/**
 * E2E test for CIRCULARIZE maneuver operation
 *
 * Tests that MechJeb can create a circularization maneuver node at apoapsis.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('CIRCULARIZE', () => {
  beforeAll(async () => {
    // Can chain after ascent - vessel is already in orbit, no reload needed
    await ensureKspReady(SAVES.ORBIT, { chainAfter: ['ascent'] });
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates circularization node at apoapsis', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Creating circularization node at apoapsis...');
    const result = await maneuver.circularize('APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates circularization node at periapsis', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Creating circularization node at periapsis...');
    const result = await maneuver.circularize('PERIAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });
});
