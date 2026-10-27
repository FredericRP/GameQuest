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
  const string TMPEssentials = "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage";

  static CIWebGLSettings()
  {
    // A fresh project lacks the TextMesh Pro essentials (default font and settings): import them once.
    // Path.GetFullPath resolves the virtual Packages/ path to the package cache.
    string essentials = Path.GetFullPath(TMPEssentials);
    if (File.Exists(essentials) && !Directory.Exists("Assets/TextMesh Pro"))
      AssetDatabase.ImportPackage(essentials, false);

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
