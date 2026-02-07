using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.UI
{
    public interface IUIToolkitViewOpener
    {
        UniTask OpenAsync(UIToolkitViewData viewData, CancellationToken cancellationToken = default);
    }
}
