﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Movement.Strategies;
using ZooWorld.Models;
using ZooWorld.Services;
using ZooWorld.Settings;
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
        private readonly IViewportBoundsService _viewportBoundsService;
        private readonly ScreenBoundsSettings _screenBoundsSettings;

        public MovementSystem(
            IAnimalsModel animalsModel,
            IAnimalViewsRegistry viewsRegistry,
            IAnimalMovementStrategyResolver strategyResolver,
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings)
        {
            _animalsModel = animalsModel;
            _viewsRegistry = viewsRegistry;
            _strategyResolver = strategyResolver;
            _viewportBoundsService = viewportBoundsService;
            _screenBoundsSettings = screenBoundsSettings;
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
            var context = new AnimalMovementContext(_viewportBoundsService, _screenBoundsSettings, Time.time);

            foreach (var animal in _animalsModel.Animals)
            {
                var strategy = _strategyResolver.Resolve(animal);

                if (!animal.IsAlive.CurrentValue)
                {
                    strategy.Cleanup(animal.Id);

                    continue;
                }

                if (!_viewsRegistry.TryGetRigidbody(animal.Id, out var rigidbody))
                {

                    continue;
                }

                strategy.Tick(animal, rigidbody, context);
            }
        }
    }
}