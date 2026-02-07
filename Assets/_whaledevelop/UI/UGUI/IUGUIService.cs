using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop.UI
{
    public interface IUGUIService : IService
    {
        bool TryGetModel<T>(ViewId viewId, out T viewModel) where T : IUIViewModel;
        
        UniTask OpenAsync<T>(UIView<T> prefab, T model, CancellationToken cancellationToken = default)
            where T : IUIViewModel;

        UniTask CloseAsync(ViewId viewId, CancellationToken cancellationToken = default);
    }
}
