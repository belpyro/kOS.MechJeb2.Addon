/**
 * E2E test for ascent guidance
 *
 * Tests that MechJeb can launch a vessel to orbit using ascent guidance.
 * This test uses the launchpad save and requires more time.
 */

import { ensureKspReady, getAscentProgram, SAVES, TIMEOUTS } from '../helpers/test-setup.js';

describe('ASCENT', () => {
  beforeAll(async () => {
    // Use launchpad save for ascent tests
    // MUST use forceReload - always reload save (via AppleScript if available), never use fast path
    // because vessel state changes from pad to orbit
    await ensureKspReady(SAVES.LAUNCHPAD, { forceReload: true });
  }, TIMEOUTS.KSP_STARTUP);

  it('launches vessel to 100km orbit', async () => {
    const ascent = await getAscentProgram();
    const targetAltitude = 100000; // 100km
    const targetInclination = 0;   // Equatorial

    console.log(`  Launching to ${targetAltitude / 1000}km x ${targetInclination} deg orbit...`);
    console.log('  This test will take several minutes.');

    // launchToOrbit returns an AscentHandle for monitoring
    const handle = await ascent.launchToOrbit({
      altitude: targetAltitude,
      inclination: targetInclination
    });

    expect(handle).toBeDefined();
    expect(handle.targetAltitude).toBe(targetAltitude);
    console.log(`  Launch initiated successfully (handle: ${handle.id})`);

    // Wait for orbit completion (uses polling)
    const result = await handle.waitForCompletion();
    expect(result.success).toBe(true);
    console.log(`  Final orbit: ${Math.round(result.finalOrbit.apoapsis / 1000)}km x ${Math.round(result.finalOrbit.periapsis / 1000)}km`);
  }, TIMEOUTS.BURN_EXECUTION);
});
