using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Movement.Strategies;
using ZooWorld.Models;
using ZooWorld.Views;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class MovementSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly IAnimalViewsRegistry _viewsRegistry;
        private readonly IAnimalMovementStrategyResolver _strategyResolver;

        public MovementSystem(
            IAnimalsModel animalsModel,
            IAnimalViewsRegistry viewsRegistry,
            IAnimalMovementStrategyResolver strategyResolver)
        {
            _animalsModel = animalsModel;
            _viewsRegistry = viewsRegistry;
            _strategyResolver = strategyResolver;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            UpdateLoopAsync(cancellationToken).Forget();

            return UniTask.CompletedTask;
        }

        private async UniTask UpdateLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                UpdateAnimals();
            }
        }

        private void UpdateAnimals()
        {
            foreach (var animal in _animalsModel.Animals)
            {
                if (!animal.IsAlive.CurrentValue)
                {
                    var cleanupStrategy = _strategyResolver.Resolve(animal);
                    cleanupStrategy.Cleanup(animal.Id);

                    continue;
                }

                if (!_viewsRegistry.TryGetRigidbody(animal.Id, out var rigidbody))
                {

                    continue;
                }

                var strategy = _strategyResolver.Resolve(animal);
                strategy.Tick(animal, rigidbody, Time.time);
            }
        }
    }
}