using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Views;
using Whaledevelop;
using Whaledevelop.Systems;
using Object = UnityEngine.Object;

namespace ZooWorld.Systems
{
    public sealed class AnimalDespawnSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly IAnimalViewsRegistry _viewsRegistry;
        private readonly List<IDisposable> _subscriptions;

        public AnimalDespawnSystem(IAnimalsModel animalsModel, IAnimalViewsRegistry viewsRegistry)
        {
            _animalsModel = animalsModel;
            _viewsRegistry = viewsRegistry;
            _subscriptions = new List<IDisposable>();
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _subscriptions.Add(_animalsModel.OnAnimalDied.Subscribe(OnAnimalDied));

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

        private void OnAnimalDied(IAnimalModel animal)
        {
            if (_viewsRegistry.TryGet(animal.Id, out var view))
            {
                Object.Destroy(view.gameObject);
            }

            _animalsModel.RemoveAnimal(animal);
        }
    }
}