using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public sealed class ScriptsUnifierWindow : OdinEditorWindow
{
    private const string SCRIPTS_BASE = "Assets/_game/Scripts";

    [MenuItem("Tools/Whaledevelop/Scripts Unifier")]
    private static void Open()
    {
        var window = GetWindow<ScriptsUnifierWindow>();
        window.titleContent = new GUIContent("Scripts Unifier");
        window.Show();
    }

    [BoxGroup("Sources")]
    [FolderPath(AbsolutePath = false)]
    [SerializeField] private List<string> _folders;

    [BoxGroup("Sources")]
    [AssetsOnly]
    [SerializeField] private List<MonoScript> _files;

    [BoxGroup("Output")]
    [Sirenix.OdinInspector.FilePath(Extensions = "txt", AbsolutePath = false)]
    [SerializeField] private string _outputTxtPath;

    [BoxGroup("Options")]
    [SerializeField] private bool _includeHeaders = true;

    [BoxGroup("Limits")]
    [MinValue(1)]
    [SerializeField] private int _symbolsLimit = 500_000;

    [BoxGroup("Actions")]
    [Button(ButtonSizes.Large)]
    private void Generate()
    {
        var candidates = CollectCandidatePaths();
        if (candidates.Count == 0)
        {
            Debug.LogError("No .cs files found in selected folders/files.");

            return;
        }

        var distinctPaths = new List<string>();
        var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in candidates)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var rel = ToProjectRelative(path);
            if (!unique.Add(rel))
            {
                continue;
            }

            distinctPaths.Add(rel);
        }

        if (distinctPaths.Count == 0)
        {
            Debug.LogError("No valid .cs files to process.");

            return;
        }

        distinctPaths.Sort(StringComparer.OrdinalIgnoreCase);

        var builder = new System.Text.StringBuilder();
        foreach (var relPath in distinctPaths)
        {
            if (_includeHeaders)
            {
                builder.Append("\n\n// ===== ").Append(relPath).Append(" =====\n");
            }

            var abs = ToAbsolute(relPath);
            var text = File.ReadAllText(abs);
            builder.Append(text);
            builder.Append('\n');
        }

        var result = builder.ToString();
        if (result.Length > _symbolsLimit)
        {
            Debug.LogError($"Result exceeds symbols limit: {result.Length} > {_symbolsLimit}. Aborting.");

            return;
        }

        if (string.IsNullOrWhiteSpace(_outputTxtPath))
        {
            Debug.LogError("Target path is not set or invalid.");

            return;
        }

        var targetAssetPath = ToProjectRelative(EnsureTxtExtension(_outputTxtPath));
        var absTarget = ToAbsolute(targetAssetPath);
        var dir = Path.GetDirectoryName(absTarget);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(absTarget, result);
        AssetDatabase.Refresh();
        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(targetAssetPath);
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
        Debug.Log($"Scripts unified into: {targetAssetPath} ({result.Length} chars)");
    }

    private List<string> CollectCandidatePaths()
    {
        var filePaths = new List<string>();

        if (_folders != null && _folders.Count > 0)
        {
            foreach (var folder in _folders)
            {
                var relBaseResolved = ResolveUnderBase(folder);
                if (string.IsNullOrWhiteSpace(relBaseResolved))
                {
                    continue;
                }

                var abs = ToAbsolute(relBaseResolved);
                if (Directory.Exists(abs))
                {
                    var found = Directory.GetFiles(abs, "*.cs", SearchOption.AllDirectories);
                    foreach (var foundFile in found)
                    {
                        var relFile = ToProjectRelative(foundFile);
                        filePaths.Add(relFile);
                    }
                }
            }
        }

        if (_files != null && _files.Count > 0)
        {
            foreach (var mono in _files)
            {
                if (mono == null)
                {
                    continue;
                }

                var path = AssetDatabase.GetAssetPath(mono);
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                filePaths.Add(path);
            }
        }

        return filePaths;
    }

    private static string EnsureTxtExtension(string path)
    {
        if (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return ToProjectRelative(path);
        }

        var withExt = path + ".txt";

        return ToProjectRelative(withExt);
    }

    private static string ResolveUnderBase(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalized = path.Replace('\\', '/');
        if (normalized.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || normalized.Contains("/Assets/", StringComparison.OrdinalIgnoreCase))
        {
            var rel = ToProjectRelative(normalized);

            return rel;
        }

        var combined = SCRIPTS_BASE.TrimEnd('/') + "/" + normalized.TrimStart('/');

        return ToProjectRelative(combined);
    }

    private static string ToProjectRelative(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalized = path.Replace('\\', '/');
        var idx = normalized.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            var rel = normalized.Substring(idx + 1);

            return rel;
        }

        if (normalized.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || string.Equals(normalized, "Assets", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        var data = Application.dataPath.Replace('\\', '/');
        var projectRoot = Path.GetDirectoryName(data)?.Replace('\\', '/');
        if (!string.IsNullOrEmpty(projectRoot) && normalized.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
        {
            var rel = "Assets" + normalized.Substring(projectRoot.Length);

            return rel.Replace('\\', '/');
        }

        return normalized;
    }

    private static string ToAbsolute(string projectRelative)
    {
        var pr = ToProjectRelative(projectRelative);
        var root = Path.GetDirectoryName(Application.dataPath);
        var abs = Path.Combine(root ?? string.Empty, pr).Replace('\\', '/');

        return abs;
    }
}
