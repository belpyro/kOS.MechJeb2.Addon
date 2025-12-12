/** @type {import('jest').Config} */
export default {
  preset: 'ts-jest/presets/default-esm',
  testEnvironment: 'node',
  testMatch: ['**/tests/**/*.test.ts'],
  testTimeout: 600000,  // 10 minutes for long-running tests
  maxWorkers: 1,        // Sequential execution (KSP can't handle parallel)
  setupFilesAfterEnv: ['./src/helpers/test-setup.ts'],
  extensionsToTreatAsEsm: ['.ts'],
  moduleNameMapper: {
    '^(\\.{1,2}/.*)\\.js$': '$1',
  },
  transform: {
    '^.+\\.tsx?$': [
      'ts-jest',
      {
        useESM: true,
      },
    ],
  },
  // Run tests in mission order (ascent, circularize, changeap, etc.)
  testSequencer: './src/mission-sequencer.cjs',
  // Verbose output to see test progress
  verbose: true,
};
