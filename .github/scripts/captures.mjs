// Lance le build WebGL dans Chromium headless (rendu logiciel) et prend des captures.
// Usage : node captures.mjs <dossier du build> <dossier de sortie>
import { chromium } from 'playwright';
import { createServer } from 'node:http';
import { readFile, mkdir } from 'node:fs/promises';
import { join, extname } from 'node:path';

const [root, out] = process.argv.slice(2);
const types = { '.html': 'text/html', '.js': 'application/javascript', '.json': 'application/json', '.wasm': 'application/wasm', '.png': 'image/png', '.css': 'text/css' };

const server = createServer(async (req, res) => {
  let path = decodeURIComponent(new URL(req.url, 'http://x').pathname);
  if (path.endsWith('/')) path += 'index.html';
  try {
    const body = await readFile(join(root, path));
    res.writeHead(200, { 'Content-Type': types[extname(path)] ?? 'application/octet-stream' });
    res.end(body);
  } catch {
    res.writeHead(404).end();
  }
}).listen(8080);

await mkdir(out, { recursive: true });
const browser = await chromium.launch({ args: ['--use-angle=swiftshader', '--enable-unsafe-swiftshader', '--ignore-gpu-blocklist'] });
const page = await browser.newPage({ viewport: { width: 960, height: 600 } });
page.on('console', (m) => console.log('[jeu]', m.text()));
await page.goto('http://localhost:8080/');

const canvas = page.locator('canvas').first();
const shot = async (name, wait) => {
  await page.waitForTimeout(wait);
  await canvas.screenshot({ path: join(out, `${name}.png`) });
  console.log('capture', name);
};

await shot('01-demarrage', 20000);
await shot('02-menu', 15000);
// Quelques entrées génériques (clic, Entrée, Espace, flèche haut), à adapter au projet.
await canvas.click();
for (const key of ['Enter', 'Space']) await page.keyboard.press(key);
await shot('03-jeu', 8000);
await canvas.click({ position: { x: 300, y: 250 } });
await page.keyboard.down('ArrowUp');
await shot('04-action', 4000);
await page.keyboard.up('ArrowUp');
await shot('05-action', 4000);

await browser.close();
server.close();
