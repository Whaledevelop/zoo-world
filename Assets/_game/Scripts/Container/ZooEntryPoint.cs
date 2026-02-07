using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop;
using Whaledevelop.VContainer;

namespace ZooWorld.Systems
{
    public sealed class ZooEntryPoint : GameEntryPointBase
    {
        private readonly IEnumerable<IAsyncInitializable> _initializables;
        private readonly IEnumerable<IAsyncReleasable> _releasables;
        private readonly IGameCycleCallbacks _gameCycleCallbacks;

        public ZooEntryPoint(
            IEnumerable<IAsyncInitializable> initializables,
            IEnumerable<IAsyncReleasable> releasables,
            IGameCycleCallbacks gameCycleCallbacks)
        {
            _initializables = initializables;
            _releasables = releasables;
            _gameCycleCallbacks = gameCycleCallbacks;
        }

        public override async Awaitable StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            foreach (var initializable in _initializables)
            {
                await initializable.InitializeAsync(cancellation);
            }

            _gameCycleCallbacks.OnDestroyEvent += Release;
            _gameCycleCallbacks.OnApplicationQuitEvent += Release;
        }

        private void Release()
        {
            ReleaseAsync().Forget();
        }

        private async UniTask ReleaseAsync()
        {
            foreach (var releasable in _releasables)
            {
                await releasable.ReleaseAsync(CancellationToken.None);
            }
        }
    }
}