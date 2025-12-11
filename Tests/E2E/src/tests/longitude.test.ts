/**
 * E2E test for LONGITUDE (Argument of Periapsis) maneuver operation
 *
 * Tests that MechJeb can create a node to change orbital longitude of periapsis.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('LONGITUDE', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates longitude change node at apoapsis', async () => {
    const maneuver = await getManeuverProgram();
    const targetLong = 45; // 45 degrees

    console.log(`  Changing longitude of periapsis to ${targetLong} degrees at apoapsis...`);
    const result = await maneuver.changeLongitude(targetLong, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates longitude change node at periapsis', async () => {
    const maneuver = await getManeuverProgram();
    const targetLong = 270;

    console.log(`  Changing longitude of periapsis to ${targetLong} degrees at periapsis...`);
    const result = await maneuver.changeLongitude(targetLong, 'PERIAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
  });
});
