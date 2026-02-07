using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using ZooWorld.Enums;
using ZooWorld.Events;
using ZooWorld.Models;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class FoodChainSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly List<IDisposable> _subscriptions;

        public FoodChainSystem(IAnimalsModel animalsModel)
        {
            _animalsModel = animalsModel;
            _subscriptions = new List<IDisposable>();
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _subscriptions.Add(_animalsModel.OnAnimalCollision.Subscribe(OnAnimalCollision));

            return UniTask.CompletedTask;
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();

            return UniTask.CompletedTask;
        }

        private void OnAnimalCollision(AnimalCollisionEvent collisionEvent)
        {
            var first = collisionEvent.First;
            var second = collisionEvent.Second;

            if (!first.IsAlive.CurrentValue || !second.IsAlive.CurrentValue)
            {

                return;
            }

            if (first.Group == AnimalGroup.Predator && second.Group == AnimalGroup.Prey)
            {
                _animalsModel.RegisterPredatorEat(first, second);
            }
            else if (second.Group == AnimalGroup.Predator && first.Group == AnimalGroup.Prey)
            {
                _animalsModel.RegisterPredatorEat(second, first);
            }
        }
    }
}