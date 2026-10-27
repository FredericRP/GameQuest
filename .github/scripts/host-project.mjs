// Creates a host Unity project around a UPM package so it can be built to WebGL.
// Usage: node host-project.mjs <package folder> <host project folder> <Unity version>
import { readFileSync, writeFileSync, mkdirSync, cpSync, readdirSync, statSync } from 'node:fs';
import { join, relative, basename } from 'node:path';

const [pkgDir, hostDir, unityVersion] = process.argv.slice(2);
const pkg = JSON.parse(readFileSync(join(pkgDir, 'package.json'), 'utf8'));
const target = join(hostDir, 'Assets', basename(pkg.name));
const skip = new Set(['.git', '.github', 'node_modules', 'build', basename(hostDir)]);

// Copy the package into Assets; Samples~ folders are renamed so Unity imports them.
mkdirSync(target, { recursive: true });
for (const entry of readdirSync(pkgDir)) {
  if (skip.has(entry)) continue;
  cpSync(join(pkgDir, entry), join(target, entry === 'Samples~' ? 'Samples' : entry), { recursive: true });
}

// Package dependencies + common built-in modules; non-Unity packages come from OpenUPM.
const modules = ['ai', 'animation', 'audio', 'imageconversion', 'imgui', 'jsonserialize', 'particlesystem',
  'physics', 'physics2d', 'screencapture', 'tilemap', 'ui', 'uielements', 'unitywebrequest', 'video'];
const dependencies = { 'com.unity.ugui': '1.0.0', ...pkg.dependencies };
for (const m of modules) dependencies[`com.unity.modules.${m}`] = '1.0.0';
const scopes = [...new Set(Object.keys(pkg.dependencies ?? {})
  .filter((name) => !name.startsWith('com.unity.'))
  .map((name) => name.split('.').slice(0, 2).join('.')))];
const manifest = { dependencies };
if (scopes.length) manifest.scopedRegistries = [{ name: 'OpenUPM', url: 'https://package.openupm.com', scopes }];
mkdirSync(join(hostDir, 'Packages'), { recursive: true });
writeFileSync(join(hostDir, 'Packages', 'manifest.json'), JSON.stringify(manifest, null, 2));

mkdirSync(join(hostDir, 'ProjectSettings'), { recursive: true });
writeFileSync(join(hostDir, 'ProjectSettings', 'ProjectVersion.txt'), `m_EditorVersion: ${unityVersion}\n`);

// Every scene of the package (in practice the sample ones) is added to the build.
const scenes = [];
const walk = (dir) => readdirSync(dir).forEach((entry) => {
  const path = join(dir, entry);
  if (statSync(path).isDirectory()) walk(path);
  else if (entry.endsWith('.unity')) scenes.push(relative(hostDir, path).replaceAll('\\', '/'));
});
walk(target);
mkdirSync(join(hostDir, 'Assets', 'Editor'), { recursive: true });
writeFileSync(join(hostDir, 'Assets', 'Editor', 'webgl-scenes.txt'), scenes.join('\n'));
console.log('Scenes:', scenes);
