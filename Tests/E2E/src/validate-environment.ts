/**
 * Environment validation for E2E tests
 *
 * Checks that all required dependencies, addons, and test assets are in place.
 * Runs before tests to provide clear error messages for missing requirements.
 */

import { existsSync, copyFileSync, mkdirSync, readdirSync, statSync } from 'fs';
import { join, dirname } from 'path';
import { fileURLToPath } from 'url';
import { config as dotenvConfig } from 'dotenv';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Load .env before checking
dotenvConfig({ path: join(__dirname, '..', '.env') });

export interface ValidationResult {
  valid: boolean;
  errors: string[];
  warnings: string[];
  assetsCopied: string[];
}

// Addon detection: CKAN package name -> actual GameData path(s)
// Some mods install to different folder names than their CKAN package name
interface AddonCheck {
  name: string;           // Display name for user
  ckanName: string;       // CKAN package name (for README)
  paths: string[];        // GameData paths to check (any match = installed)
  required: boolean;      // If false, warn instead of error
}

const ADDON_CHECKS: AddonCheck[] = [
  { name: 'kOS', ckanName: 'kOS', paths: ['kOS'], required: true },
  { name: 'MechJeb2', ckanName: 'MechJeb2', paths: ['MechJeb2'], required: true },
  { name: 'KSP-AutoLoad', ckanName: 'KSP-AutoLoad', paths: ['KSP-AutoLoad'], required: true },
  { name: 'kOS.MechJeb2.Addon', ckanName: 'kOS.MechJeb2.Addon', paths: ['kOS.MechJeb2.Addon'], required: true },
  // ForAll mods - optional, but recommended for testing on any vessel
  { name: 'MechJebForAll', ckanName: 'MechJebForAll', paths: ['MechJebForAll'], required: false },
  // kOSforAll installs to XyphosAerospace, not a folder named kOSforAll!
  // Use join() to create platform-appropriate paths
  { name: 'kOSforAll', ckanName: 'kOSforAll', paths: [join('XyphosAerospace', 'Tweaks', 'kOS-CommandPod')], required: false },
];

/**
 * Validate the test environment
 *
 * Checks:
 * 1. KSP_DIR environment variable
 * 2. node_modules and ksp-mcp package
 * 3. Required KSP addons in GameData
 * 4. Copies test assets if missing (without overwriting)
 */
export async function validateEnvironment(): Promise<ValidationResult> {
  const result: ValidationResult = {
    valid: true,
    errors: [],
    warnings: [],
    assetsCopied: []
  };

  // 1. Check KSP_DIR environment variable
  const KSP_DIR = process.env.KSP_DIR;
  if (!KSP_DIR) {
    const isWindows = process.platform === 'win32';
    const examplePath = isWindows
      ? 'C:\\Program Files (x86)\\Steam\\steamapps\\common\\Kerbal Space Program'
      : '/path/to/Kerbal Space Program';
    result.errors.push(
      'KSP_DIR environment variable not set.\n' +
      `   Create Tests/E2E/.env with: KSP_DIR=${examplePath}`
    );
    result.valid = false;
    return result; // Can't continue without this
  }

  if (!existsSync(KSP_DIR)) {
    result.errors.push(`KSP_DIR path does not exist: ${KSP_DIR}`);
    result.valid = false;
    return result;
  }

  // 2. Check node_modules
  const nodeModulesPath = join(__dirname, '..', 'node_modules');
  if (!existsSync(nodeModulesPath)) {
    result.errors.push('node_modules not found. Run: npm install');
    result.valid = false;
  }

  // Check ksp-mcp package specifically
  const kspMcpPath = join(nodeModulesPath, 'ksp-mcp');
  if (existsSync(nodeModulesPath) && !existsSync(kspMcpPath)) {
    result.errors.push(
      'ksp-mcp package not installed.\n' +
      '   This package provides KosConnection, ManeuverProgram, and AscentProgram.\n' +
      '   Run: npm install'
    );
    result.valid = false;
  }

  // 3. Check addons (required and optional)
  const gameDataPath = join(KSP_DIR, 'GameData');
  for (const addon of ADDON_CHECKS) {
    // Check if any of the paths exist
    const installed = addon.paths.some(p => existsSync(join(gameDataPath, p)));

    if (!installed) {
      if (addon.required) {
        const checkedPaths = addon.paths.map(p => join(gameDataPath, p)).join('\n   or: ');
        result.errors.push(`Missing required addon: ${addon.name} (CKAN: ${addon.ckanName})\n   Expected at: ${checkedPaths}`);
        result.valid = false;
      } else {
        result.warnings.push(`Optional addon not installed: ${addon.name} (CKAN: ${addon.ckanName})`);
      }
    }
  }

  // 4. Copy test assets if needed (only if KSP_DIR is valid)
  if (existsSync(KSP_DIR)) {
    await copyTestAssets(KSP_DIR, result);
  }

  return result;
}

/**
 * Copy test assets to KSP directories (without overwriting existing files)
 */
async function copyTestAssets(kspDir: string, result: ValidationResult): Promise<void> {
  const assetDir = join(__dirname, '..', 'asset');
  const stockSaveSource = join(assetDir, 'stock');
  const autoLoadSource = join(assetDir, 'AutoLoad.cfg');

  // Copy save files to KSP saves folder
  const kspSavesDir = join(kspDir, 'saves', 'stock');
  if (existsSync(stockSaveSource)) {
    copyDirectoryWithWarnings(stockSaveSource, kspSavesDir, result);
  }

  // Copy AutoLoad.cfg
  const autoLoadDestDir = join(kspDir, 'GameData', 'KSP-AutoLoad');
  const autoLoadDest = join(autoLoadDestDir, 'AutoLoad.cfg');
  if (existsSync(autoLoadSource)) {
    if (existsSync(autoLoadDest)) {
      // File already exists - this is fine, don't warn
    } else if (existsSync(autoLoadDestDir)) {
      // Only copy if the KSP-AutoLoad directory exists (mod is installed)
      copyFileSync(autoLoadSource, autoLoadDest);
      result.assetsCopied.push(autoLoadDest);
    }
  }
}

/**
 * Recursively copy directory, warning about existing files
 */
function copyDirectoryWithWarnings(src: string, dest: string, result: ValidationResult): void {
  if (!existsSync(dest)) {
    mkdirSync(dest, { recursive: true });
  }

  const entries = readdirSync(src);
  for (const entry of entries) {
    // Skip hidden files (like .DS_Store)
    if (entry.startsWith('.')) {
      continue;
    }

    const srcPath = join(src, entry);
    const destPath = join(dest, entry);
    const stat = statSync(srcPath);

    if (stat.isDirectory()) {
      copyDirectoryWithWarnings(srcPath, destPath, result);
    } else {
      // Only copy if destination doesn't exist (don't overwrite user files)
      if (!existsSync(destPath)) {
        copyFileSync(srcPath, destPath);
        result.assetsCopied.push(destPath);
      }
    }
  }
}

/**
 * Format validation result for console output
 */
export function formatValidationResult(result: ValidationResult): string {
  const lines: string[] = [];

  lines.push('\n=== E2E Environment Validation ===\n');

  if (result.errors.length > 0) {
    lines.push('ERRORS:');
    result.errors.forEach(e => lines.push(`  X ${e}`));
  }

  if (result.warnings.length > 0) {
    lines.push('\nWARNINGS:');
    result.warnings.forEach(w => lines.push(`  ! ${w}`));
  }

  if (result.assetsCopied.length > 0) {
    lines.push('\nASSETS COPIED:');
    result.assetsCopied.forEach(a => lines.push(`  + ${a}`));
  }

  lines.push('\n' + (result.valid ? '>>> Environment valid!' : '>>> Environment invalid - see errors above'));

  return lines.join('\n');
}

// CLI runner - check if this file is being run directly
const isMainModule = process.argv[1] && (
  process.argv[1].endsWith('validate-environment.ts') ||
  process.argv[1].endsWith('validate-environment.js')
);

if (isMainModule) {
  validateEnvironment().then(result => {
    console.log(formatValidationResult(result));
    process.exit(result.valid ? 0 : 1);
  });
}
