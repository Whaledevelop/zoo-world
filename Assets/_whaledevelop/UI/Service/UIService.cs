using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using QuizRPG;
using UnityEngine;
using VContainer;
using Whaledevelop.Services;
using Object = UnityEngine.Object;

namespace Whaledevelop.UI
{
    public class UIService : Service, IUIService
    {
        private readonly IUISettings _settings;

        private readonly List<Canvas> _canvasInstances = new();
        private readonly List<RectTransform> _canvasRoots = new();

        private readonly Dictionary<int, UIViewRuntimeData> _viewsDict = new();
        private readonly Dictionary<int, RootUIViewRuntimeData> _rootViewsDict = new();

        private IObjectResolver _resolver;

        public UIService(IUISettings settings)
        {
            _settings = settings;
        }

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _canvasInstances.Clear();
            _canvasRoots.Clear();
            var canvasPrefabs = _settings.CanvasPrefabs;
            if (canvasPrefabs.Count == 0)
            {
                Debug.LogError("UIService - no canvas prefabs configured in settings");

                return UniTask.CompletedTask;
            }
            foreach (var canvasPrefab in canvasPrefabs)
            {
                var canvasInstance = Object.Instantiate(canvasPrefab);
                Object.DontDestroyOnLoad(canvasInstance);
                _canvasInstances.Add(canvasInstance);
                var parent = canvasInstance.transform as RectTransform;
                _canvasRoots.Add(parent);
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            CloseAll();

            foreach (var canvasInstance in _canvasInstances)
            {
                if (canvasInstance)
                {
                    Object.Destroy(canvasInstance.gameObject);
                }
            }

            _canvasInstances.Clear();
            _canvasRoots.Clear();

            _viewsDict.Clear();
            _rootViewsDict.Clear();

            return UniTask.CompletedTask;
        }

        public bool TryGetRoot(int rootIndex, out RootUIView root)
        {
            if (_rootViewsDict.TryGetValue(rootIndex, out var rootUIViewRuntimeData))
            {
                root = rootUIViewRuntimeData.View;
                return true;
            }
            root = null;
            return false;
        }

        public bool TryGetModel<TModel>(int viewIndex, out TModel model) where TModel : IUIViewModel
        {
            if (!_viewsDict.TryGetValue(viewIndex, out var viewData) || viewData.Model is not TModel typedModel)
            {
                model = default;

                return false;
            }
            model = typedModel;

            return true;
        }
        
        public bool TryGetModelOfType<TModel>(out TModel model)
        {
            foreach (var (_, runtimeData) in _viewsDict)
            {
                if (runtimeData.Model is TModel typedModel)
                {
                    model = typedModel;
                    return true;
                }
            }
            model = default;
            return false;
        }

        public bool TryGetView<TView>(int viewIndex, out TView view) where TView : UIView
        {
            if (!_viewsDict.TryGetValue(viewIndex, out var viewData) || viewData.View is not TView typedView)
            {
                view = default;

                return false;
            }

            view = typedView;

            return true;
        }

        public bool TryOpenView(int viewIndex, IUIViewModel viewModel, int rootIndex = -1, int canvasIndex = 0)
        {
            if (_viewsDict.ContainsKey(viewIndex))
            {

                return false;
            }
            if (!TryGetViewPrefab(viewIndex, out var viewPrefab))
            {

                return false;
            }
            RectTransform parent;
            int? rootViewIndex = null;
            if (rootIndex >= 0)
            {
                if (!_rootViewsDict.TryGetValue(rootIndex, out var rootViewData))
                {
                    Debug.LogError($"No root view with index {rootIndex} for sub view index {viewIndex}");
                    return false;
                }

                if (!rootViewData.View.TryGetUIViewContainer(viewPrefab, out var placementPoint))
                {
                    Debug.LogError($"Cant get container for view index {viewIndex} from root index {rootIndex}");
                    return false;
                }
                parent = placementPoint;
                rootViewIndex = rootIndex;
            }
            else
            {
                if (!TryGetCanvasParent(canvasIndex, out var canvasParent))
                {

                    return false;
                }

                parent = canvasParent;
            }
            OpenView(viewIndex, viewPrefab, parent, viewModel, rootViewIndex);

            OnViewOpened?.Invoke(viewIndex);
            
            return true;
        }

        public bool TryCloseView(int viewIndex)
        {
            if (!_viewsDict.TryGetValue(viewIndex, out var viewData))
            {

                return false;
            }
            ReleaseUIView(viewData);
            _viewsDict.Remove(viewIndex);
            
            OnViewClosed?.Invoke(viewIndex);
            
            return true;
        }

        public bool TryOpenRoot(int rootIndex, int canvasIndex = 0)
        {
            if (_rootViewsDict.ContainsKey(rootIndex))
            {

                return false;
            }
            if (!TryGetRootPrefab(rootIndex, out var rootPrefab))
            {

                return false;
            }
            if (!TryGetCanvasParent(canvasIndex, out var parent))
            {

                return false;
            }

            var rootInstance = Object.Instantiate(rootPrefab, parent);
            var runtimeData = new RootUIViewRuntimeData(rootIndex, rootInstance, canvasIndex);
            _rootViewsDict.Add(rootIndex, runtimeData);

            return true;
        }

        public bool TryCloseRoot(int rootIndex)
        {
            if (!_rootViewsDict.TryGetValue(rootIndex, out var rootRuntimeData))
            {

                return false;
            }
            ReleaseRootView(rootRuntimeData);
            _rootViewsDict.Remove(rootIndex);

            return true;
        }

        public event Action<int> OnViewOpened;
        public event Action<int> OnViewClosed;

        private bool TryGetViewPrefab(int viewIndex, out UIView viewPrefab)
        {
            var prefabs = _settings.UIViewsPrefabs;
            if (viewIndex < 0 || viewIndex >= prefabs.Count)
            {
                Debug.LogError($"UIService.OpenView - prefab for view index '{viewIndex}' is not assigned.");
                viewPrefab = null;

                return false;
            }
            viewPrefab = prefabs[viewIndex];
            if (viewPrefab == null)
            {
                Debug.LogError($"UIService.OpenView - prefab for view index '{viewIndex}' is not assigned.");
                viewPrefab = null;

                return false;
            }

            return true;
        }

        private bool TryGetRootPrefab(int rootIndex, out RootUIView rootPrefab)
        {
            var prefabs = _settings.RootsPrefabs;
            if (rootIndex < 0 || rootIndex >= prefabs.Count)
            {
                Debug.LogError($"UIService.OpenRoot - prefab for root index '{rootIndex}' is not assigned.");
                rootPrefab = null;

                return false;
            }
            rootPrefab = prefabs[rootIndex];
            if (rootPrefab == null)
            {
                Debug.LogError($"UIService.OpenRoot - prefab for root index '{rootIndex}' is not assigned.");
                rootPrefab = null;

                return false;
            }

            return true;
        }

        private void OpenView(int viewIndex, UIView viewPrefab, RectTransform parent, IUIViewModel viewModel, int? rootIndex = null)
        {
            var viewInstance = Object.Instantiate(viewPrefab, parent);
            
            viewInstance.InitializeAndInjectModel(_resolver, viewModel);
            
            var runtimeData = new UIViewRuntimeData(viewIndex, viewModel, viewInstance, rootIndex);
            _viewsDict.Add(viewIndex, runtimeData);
            if (rootIndex.HasValue && _rootViewsDict.TryGetValue(rootIndex.Value, out var rootRuntimeData))
            {
                rootRuntimeData.SubViewIndexes.Add(viewIndex);
            }
        }

        private void ReleaseUIView(UIViewRuntimeData runtimeData)
        {
            runtimeData.View.Release();
            Object.Destroy(runtimeData.View.gameObject);
            if (runtimeData.RootViewIndex.HasValue && _rootViewsDict.TryGetValue(runtimeData.RootViewIndex.Value, out var rootRuntimeData))
            {
                rootRuntimeData.SubViewIndexes.Remove(runtimeData.Index);
            }
        }

        private void ReleaseRootView(RootUIViewRuntimeData runtimeData)
        {
            for (var index = runtimeData.SubViewIndexes.Count - 1; index >= 0; index--)
            {
                var subViewIndex = runtimeData.SubViewIndexes[index];
                TryCloseView(subViewIndex);
            }
            Object.Destroy(runtimeData.View.gameObject);
            runtimeData.SubViewIndexes.Clear();
        }

        private void CloseAll()
        {
            foreach (var (_, viewData) in _viewsDict)
            {
                ReleaseUIView(viewData);
            }
            _viewsDict.Clear();
            foreach (var (_, rootData) in _rootViewsDict)
            {
                ReleaseRootView(rootData);
            }
            _rootViewsDict.Clear();
        }

        private bool TryGetCanvasParent(int canvasIndex, out RectTransform parent)
        {
            if (canvasIndex < 0 || canvasIndex >= _canvasRoots.Count)
            {
                Debug.LogError($"UIService - canvas index '{canvasIndex}' is out of range");
                parent = null;

                return false;
            }

            parent = _canvasRoots[canvasIndex];

            if (!parent)
            {
                Debug.LogError($"UIService - canvas at index '{canvasIndex}' is missing RectTransform");
                parent = null;

                return false;
            }

            return true;
        }
        
        private readonly struct UIViewRuntimeData
        {
            public UIViewRuntimeData(int index, IUIViewModel model, UIView view, int? rootViewIndex = null)
            {
                Index = index;
                Model = model;
                View = view;
                RootViewIndex = rootViewIndex;
            }

            public int Index { get; }
            public IUIViewModel Model { get; }
            public UIView View { get; }
            public int? RootViewIndex { get; }
        }

        private readonly struct RootUIViewRuntimeData
        {
            public RootUIViewRuntimeData(int index, RootUIView view, int canvasIndex)
            {
                Index = index;
                View = view;
                CanvasIndex = canvasIndex;
                SubViewIndexes = new List<int>();
            }

            public int Index { get; }
            public RootUIView View { get; }
            public int CanvasIndex { get; }
            public List<int> SubViewIndexes { get; }
        }
    }
}
