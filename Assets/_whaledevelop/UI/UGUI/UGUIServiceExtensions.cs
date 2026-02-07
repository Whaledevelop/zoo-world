using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.UI
{
    public static class UGUIServiceExtensions
    {
        public static bool TryGetModel<T>(this IUGUIService service, UIView uiView, out T viewModel)
            where T : IUIViewModel
        {
            return service.TryGetModel<T>(uiView.ViewId, out viewModel);
        }
        
        public static UniTask CloseAsync(this IUGUIService service, UIView uiView, CancellationToken cancellationToken = default)
        {
            return service.CloseAsync(uiView.ViewId, cancellationToken);
        }
    }
}