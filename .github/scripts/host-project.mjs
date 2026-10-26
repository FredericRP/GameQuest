// Crée un projet Unity hôte autour d'un package UPM pour pouvoir le compiler en WebGL.
// Usage : node host-project.mjs <dossier du package> <dossier du projet hôte> <version Unity>
import { readFileSync, writeFileSync, mkdirSync, cpSync, readdirSync, statSync } from 'node:fs';
import { join, relative, basename } from 'node:path';

const [pkgDir, hostDir, unityVersion] = process.argv.slice(2);
const pkg = JSON.parse(readFileSync(join(pkgDir, 'package.json'), 'utf8'));
const target = join(hostDir, 'Assets', basename(pkg.name));
const skip = new Set(['.git', '.github', 'node_modules', 'build', basename(hostDir)]);

// Copie du package dans Assets ; les dossiers Samples~ sont renommés pour être importés.
mkdirSync(target, { recursive: true });
for (const entry of readdirSync(pkgDir)) {
  if (skip.has(entry)) continue;
  cpSync(join(pkgDir, entry), join(target, entry === 'Samples~' ? 'Samples' : entry), { recursive: true });
}

// Dépendances du package + modules intégrés courants ; les paquets non Unity viennent d'OpenUPM.
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

// Toutes les scènes du package (en pratique celles des exemples) sont ajoutées au build.
const scenes = [];
const walk = (dir) => readdirSync(dir).forEach((entry) => {
  const path = join(dir, entry);
  if (statSync(path).isDirectory()) walk(path);
  else if (entry.endsWith('.unity')) scenes.push(relative(hostDir, path).replaceAll('\\', '/'));
});
walk(target);
mkdirSync(join(hostDir, 'Assets', 'Editor'), { recursive: true });
writeFileSync(join(hostDir, 'Assets', 'Editor', 'webgl-scenes.txt'), scenes.join('\n'));
console.log('Scènes :', scenes);
