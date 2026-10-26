#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;

// Copié dans Assets/Editor uniquement pendant le build CI (jamais commité dans le projet).
// Rend le build WebGL lisible sur GitHub Pages et choisit les scènes à compiler.
[InitializeOnLoad]
static class CIWebGLSettings
{
  const string ScenesFile = "Assets/Editor/webgl-scenes.txt";

  static CIWebGLSettings()
  {
#if UNITY_2020_1_OR_NEWER
    // GitHub Pages n'envoie pas d'en-tête Content-Encoding : le loader décompresse lui-même.
    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
    PlayerSettings.WebGL.decompressionFallback = true;
#endif
    if (File.Exists(ScenesFile))
      EditorBuildSettings.scenes = File.ReadAllLines(ScenesFile)
        .Where(line => line.Trim().Length > 0)
        .Select(path => new EditorBuildSettingsScene(path.Trim(), true))
        .ToArray();
  }
}
#endif
