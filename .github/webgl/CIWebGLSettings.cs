#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;

// Copied into Assets/Editor during the CI build only (never committed to the project).
// Makes the WebGL build loadable on GitHub Pages and selects the scenes to build.
[InitializeOnLoad]
static class CIWebGLSettings
{
  const string ScenesFile = "Assets/Editor/webgl-scenes.txt";

  static CIWebGLSettings()
  {
#if UNITY_2020_1_OR_NEWER
    // GitHub Pages sends no Content-Encoding header: the loader decompresses by itself.
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
