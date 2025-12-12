/**
 * E2E test for LAN (Longitude of Ascending Node) maneuver operation
 *
 * Tests that MechJeb can create a node to change orbital plane LAN.
 */

import { ensureKspReady, getManeuverProgram, clearNodes, SAVES } from '../helpers/test-setup.js';

describe('LAN', () => {
  beforeAll(async () => {
    await ensureKspReady(SAVES.ORBIT);
  });

  beforeEach(async () => {
    await clearNodes();
  });

  it('creates LAN change node at apoapsis', async () => {
    const maneuver = await getManeuverProgram();
    const targetLAN = 90; // 90 degrees

    console.log(`  Changing LAN to ${targetLAN} degrees at apoapsis...`);
    const result = await maneuver.changeLAN(targetLAN, 'APOAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();
    expect(result.deltaV).toBeGreaterThan(0);

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
    console.log(`  Time to node: ${result.timeToNode?.toFixed(0)}s`);
  });

  it('creates LAN change node at periapsis', async () => {
    const maneuver = await getManeuverProgram();
    const targetLAN = 180;

    console.log(`  Changing LAN to ${targetLAN} degrees at periapsis...`);
    const result = await maneuver.changeLAN(targetLAN, 'PERIAPSIS');

    expect(result.success).toBe(true);
    expect(result.deltaV).toBeDefined();

    console.log(`  Node created: ${result.deltaV?.toFixed(1)} m/s`);
  });
});
