using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop
{
    public interface IInitializableAsync
    {
        bool Initialized { get; }

        UniTask InitializeAsync(CancellationToken cancellationToken);

        UniTask ReleaseAsync(CancellationToken cancellationToken);
    }
}