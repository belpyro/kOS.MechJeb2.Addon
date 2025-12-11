/**
 * Custom Jest sequencer to run tests in mission order
 *
 * Tests are ordered to simulate a real space mission:
 * 1. Launch & orbit establishment (ascent, circularize)
 * 2. Basic orbit adjustments (changeap, changepe, ellipticize)
 * 3. Plane changes (changeinclination, lan, longitude)
 * 4. Interplanetary transfer (hohmann, coursecorrection)
 * 5. Rendezvous operations (killrelvel)
 * 6. Utility maneuvers (resonant)
 */

const Sequencer = require('@jest/test-sequencer').default;

// Mission order - matches bash validate-all-tests.sh
const MISSION_ORDER = [
  'ascent',
  'circularize',
  'changeap',
  'changepe',
  'ellipticize',
  'changeinclination',
  'lan',
  'longitude',
  'hohmann',
  'coursecorrection',
  'killrelvel',
  'resonant',
];

class MissionSequencer extends Sequencer {
  sort(tests) {
    // Sort tests by their position in MISSION_ORDER
    return [...tests].sort((a, b) => {
      const aName = this.getTestName(a.path);
      const bName = this.getTestName(b.path);

      const aIndex = MISSION_ORDER.indexOf(aName);
      const bIndex = MISSION_ORDER.indexOf(bName);

      // If test not in mission order, put it at the end
      const aOrder = aIndex === -1 ? MISSION_ORDER.length : aIndex;
      const bOrder = bIndex === -1 ? MISSION_ORDER.length : bIndex;

      return aOrder - bOrder;
    });
  }

  getTestName(path) {
    // Extract test name from path like "src/tests/circularize.test.ts"
    const match = path.match(/\/([^/]+)\.test\.ts$/);
    return match ? match[1] : '';
  }
}

module.exports = MissionSequencer;
