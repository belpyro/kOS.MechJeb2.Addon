/**
 * E2E test for RESONANTORBIT maneuver operation
 *
 * Tests that MechJeb can create a node to establish a resonant orbit
 * (useful for satellite deployment constellations).
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('RESONANTORBIT', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates 2:1 resonant orbit node at apoapsis', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Creating 2:1 resonant orbit node at apoapsis...');
    const result = await maneuver.resonantOrbit(2, 1, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates 3:2 resonant orbit node at periapsis', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Creating 3:2 resonant orbit node at periapsis...');
    const result = await maneuver.resonantOrbit(3, 2, 'PERIAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
  });

  it('creates 4:3 resonant orbit for fine constellation spacing', async () => {
    const maneuver = await getManeuverProgram();

    console.log('  Creating 4:3 resonant orbit node...');
    const result = await maneuver.resonantOrbit(4, 3, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
  });
});
