// Assets/_whaledevelop/UI/Editor/UIHelperWindow.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using QuizRPG;
using Whaledevelop.UI;
using UnityEngine.UI;

namespace Whaledevelop
{
    public class UIHelperWindow : OdinEditorWindow
    {
        [SerializeField]
        private string _codesClassesNamespace;

        [SerializeField]
        [BoxGroup("Settings")]
        private UISettings _uiSettings;

        [SerializeField]
        [BoxGroup("Canvas")]
        [FolderPath(AbsolutePath = false)]
        private string _canvasPrefabsFolderPath;

        [SerializeField]
        [BoxGroup("Canvas")]
        [Sirenix.OdinInspector.FilePath(Extensions = "cs", AbsolutePath = false)]
        private string _canvasCodesFilePath;

        [SerializeField]
        [BoxGroup("UI Views")]
        [FolderPath(AbsolutePath = false)]
        private string _uiViewsPrefabsFolderPath;
        
        [SerializeField]
        [BoxGroup("UI Views")]
        [Sirenix.OdinInspector.FilePath(Extensions = "cs", AbsolutePath = false)]
        private string _uiViewCodesFilePath;

        [SerializeField] 
        [BoxGroup("Root Views")] 
        [FolderPath(AbsolutePath = false)]
        private string _rootViewsPrefabsFolderPath;
        
        [SerializeField]
        [BoxGroup("Root Views")]
        [Sirenix.OdinInspector.FilePath(Extensions = "cs", AbsolutePath = false)]
        private string _rootViewCodesFilePath;
        
        [MenuItem("Tools/Whaledevelop/UIHelper")]
        private static void OpenWindow()
        {
            var window = GetWindow<UIHelperWindow>();
            window.titleContent = new GUIContent("UIHelper");
            window.Show();
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("UI Views")]
        private void GenerateUICodes()
        {
            GenerateCodesFile<UIView>(_uiViewsPrefabsFolderPath, _uiViewCodesFilePath, "UIView", ApplyUIViewPrefabs);
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("Root Views")]
        private void GenerateRootCodes()
        {
            GenerateCodesFile<RootUIView>(_rootViewsPrefabsFolderPath, _rootViewCodesFilePath, "RootUIView", ApplyRootPrefabs);
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("Canvas")]
        private void GenerateCanvasCodes()
        {
            GenerateCodesFile<Canvas>(_canvasPrefabsFolderPath, _canvasCodesFilePath, "Canvas", ApplyCanvasPrefabs);
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("Canvas")]
        private void CollectCanvases()
        {
            if (string.IsNullOrEmpty(_canvasPrefabsFolderPath))
            {
                Debug.LogError("Canvas folder path is not assigned");

                return;
            }

            var prefabsData = AssetsUtility.LoadPrefabsWithComponent<Canvas>(_canvasPrefabsFolderPath);
            if (prefabsData.Count == 0)
            {
                Debug.LogError("No canvases found in the folder");

                return;
            }

            var canvases = new List<Canvas>();
            for (var index = 0; index < prefabsData.Count; index++)
            {
                var data = prefabsData[index];
                var scaler = AssetDatabase.LoadAssetAtPath<CanvasScaler>(data.path);
                if (scaler == null)
                {
                    continue;
                }

                canvases.Add(data.view);
            }

            if (canvases.Count == 0)
            {
                Debug.LogError("No canvases with CanvasScaler found in the folder");

                return;
            }

            ApplyCanvasPrefabs(canvases.ToArray());
        }

        protected override void OnEnable()
        {
            EditorPrefsReflectionUtility.Load(this);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            EditorPrefsReflectionUtility.Save(this);
            base.OnDisable();
        }

        private void GenerateCodesFile<TView>(string prefabsFolderPath, string codesClassPath, string postFix, Action<TView[]> assignPrefabs) where TView : Component
        {
            var prefabsData = AssetsUtility.LoadPrefabsWithComponent<TView>(prefabsFolderPath);
            if (prefabsData.Count == 0)
            {
                Debug.LogError("No prefabs found in the folder");

                return;
            }
            var entries = new List<FieldEntry>();
            var views = new TView[prefabsData.Count];
            for (var i = 0; i < prefabsData.Count; i++)
            {
                var data = prefabsData[i];
                views[i] = data.view;
                var path = data.path;
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName.EndsWith(postFix, StringComparison.Ordinal))
                {
                    fileName = fileName.Substring(0, fileName.Length - postFix.Length);
                }
                entries.Add(new FieldEntry(fileName, i));
            }

            if (entries.Count == 0)
            {
                Debug.LogError("No matching prefabs found in UISettings");

                return;
            }

            entries.Sort((x, y) =>
            {
                var compareIndex = x.Index.CompareTo(y.Index);
                return compareIndex != 0 ? compareIndex : string.CompareOrdinal(x.Name, y.Name);
            });
            assignPrefabs?.Invoke(views);
            var codesClassName = Path.GetFileNameWithoutExtension(codesClassPath);
            var contents = GenerateCodesFileContent(entries, codesClassName, _codesClassesNamespace);
            File.WriteAllText(codesClassPath, contents);
            AssetDatabase.ImportAsset(codesClassPath);

            assignPrefabs?.Invoke(views);
            Debug.Log($"{codesClassName} has been updated");
        }

        private void ApplyUIViewPrefabs(UIView[] prefabs)
        {
            if (_uiSettings == null)
            {
                Debug.LogError("UISettings is not assigned");

                return;
            }
            _uiSettings.SetUIViewsPrefabs(prefabs);
            EditorUtility.SetDirty(_uiSettings);
            AssetDatabase.SaveAssets();
        }

        private void ApplyRootPrefabs(RootUIView[] prefabs)
        {
            if (_uiSettings == null)
            {
                Debug.LogError("UISettings is not assigned");

                return;
            }
            _uiSettings.SetRootViewsPrefabs(prefabs);
            EditorUtility.SetDirty(_uiSettings);
            AssetDatabase.SaveAssets();
        }

        private void ApplyCanvasPrefabs(Canvas[] prefabs)
        {
            if (_uiSettings == null)
            {
                Debug.LogError("UISettings is not assigned");

                return;
            }

            _uiSettings.SetCanvasPrefabs(prefabs);
            EditorUtility.SetDirty(_uiSettings);
            AssetDatabase.SaveAssets();
        }

        private static string GenerateCodesFileContent(List<FieldEntry> entries, string className, string namespaceName)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"namespace {namespaceName}");
            builder.AppendLine("{");
            builder.AppendLine($"    public static class {className}");
            builder.AppendLine("    {");

            for (var index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                builder.Append("        public static int ");
                builder.Append(entry.Name);
                builder.Append(" = ");
                builder.Append(entry.Index);
                builder.AppendLine(";");
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private readonly struct FieldEntry
        {
            public FieldEntry(string name, int index)
            {
                Name = name;
                Index = index;
            }

            public string Name { get; }

            public int Index { get; }
        }
    }
}
#endif
