using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Whaledevelop.UI
{
    public class UGUIService : SyncLifetime, IUGUIService
    {
        private readonly Canvas _canvasPrefab;

        private Canvas _canvasInstance;
        
        private readonly List<UGUIViewRuntimeData> _viewRuntimeDatas = new();
        
        private IObjectResolver _objectResolver;

        public UGUIService(Canvas canvasPrefab)
        {
            _canvasPrefab = canvasPrefab;
        }

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override void OnInitialize()
        {
            _canvasInstance = Object.Instantiate(_canvasPrefab);
            Object.DontDestroyOnLoad(_canvasInstance.gameObject);
        }

        public bool TryGetModel<T>(ViewId viewId, out T viewModel) where T : IUIViewModel
        {
            var runtimeData = _viewRuntimeDatas.FirstOrDefault(data => data.ViewId.Equals(viewId));
            if (runtimeData?.ViewModel is T typedModel)
            {
                viewModel = typedModel;
                return true;
            }
            viewModel = default(T);
            return false;
        }

        public async UniTask OpenAsync<T>(UIView<T> prefab, T model, CancellationToken cancellationToken = default) where T : IUIViewModel
        {
            var runtimeData = _viewRuntimeDatas.FirstOrDefault(data => data.ViewId.Equals(prefab.ViewId));
            if (runtimeData != null)
            {
                Debug.Log("Already opened");
                return;
            }
                        
            _objectResolver.Inject(model);
            await model.InitializeAsync(cancellationToken);
            
            var instance = Object.Instantiate(prefab, _canvasInstance.transform);
            instance.Model = model;
            
            await instance.InitializeAsync(cancellationToken);
            
            runtimeData = new UGUIViewRuntimeData(prefab, instance, model);
            _viewRuntimeDatas.Add(runtimeData);
        }

        public async UniTask CloseAsync(ViewId viewId, CancellationToken cancellationToken = default)
        {
            var runtimeData = _viewRuntimeDatas.FirstOrDefault(data => data.ViewId.Equals(viewId));
            if (runtimeData == null)
            {
                Debug.Log("Already closed");
                return;
            }
            var view = runtimeData.Instance;
            await view.ReleaseAsync(cancellationToken);
            await runtimeData.ViewModel.ReleaseAsync(cancellationToken);
            Object.Destroy(view);
            _viewRuntimeDatas.Remove(runtimeData);
        }
    }
}
