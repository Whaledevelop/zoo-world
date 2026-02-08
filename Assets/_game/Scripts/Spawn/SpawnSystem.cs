﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;
using ZooWorld.Views;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class SpawnSystem : AsyncLifetime, IGameSystem
    {
        private readonly ZooWorldSettings _zooWorldSettings;
        private readonly AnimalSettingsTable _animalSettingsTable;
        private readonly IAnimalsModel _animalsModel;
        private readonly IAnimalViewsRegistry _viewsRegistry;
        private readonly ZooWorldRootView _worldRootView;
        private readonly PoolingSettings _poolingSettings;
        private readonly IAnimalViewsPool _animalViewsPool;
        private int _nextId;

        public SpawnSystem(
            ZooWorldSettings zooWorldSettings,
            AnimalSettingsTable animalSettingsTable,
            IAnimalsModel animalsModel,
            IAnimalViewsRegistry viewsRegistry,
            ZooWorldRootView worldRootView,
            PoolingSettings poolingSettings,
            IAnimalViewsPool animalViewsPool)
        {
            _zooWorldSettings = zooWorldSettings;
            _animalSettingsTable = animalSettingsTable;
            _animalsModel = animalsModel;
            _viewsRegistry = viewsRegistry;
            _worldRootView = worldRootView;
            _poolingSettings = poolingSettings;
            _animalViewsPool = animalViewsPool;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            SpawnLoopAsync(cancellationToken).Forget();

            return UniTask.CompletedTask;
        }

        private async UniTask SpawnLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                SpawnAnimal();

                var interval = Random.Range(_zooWorldSettings.SpawnIntervalRange.x, _zooWorldSettings.SpawnIntervalRange.y);
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: cancellationToken);
            }
        }

        private void SpawnAnimal()
        {
            if (IsAliveLimitReached())
            {

                return;
            }

            var definition = _animalSettingsTable.Animals[Random.Range(0, _animalSettingsTable.Animals.Count)];
            var spawnPosition = GetRandomPosition(_zooWorldSettings.WorldBounds);
            var model = new AnimalModel(++_nextId, definition, spawnPosition);
            var view = _animalViewsPool.Get(definition, _worldRootView.AnimalsSpawnRoot, spawnPosition, Quaternion.identity);

            _animalsModel.AddAnimal(model);
            view.Initialize(model, _animalsModel, _viewsRegistry);
        }

        private Vector3 GetRandomPosition(Bounds bounds)
        {
            var xPosition = Random.Range(bounds.min.x, bounds.max.x);
            var zPosition = Random.Range(bounds.min.z, bounds.max.z);

            
            return new Vector3(xPosition, bounds.center.y, zPosition);
        }

        private bool IsAliveLimitReached()
        {
            var aliveCount = 0;

            foreach (var animal in _animalsModel.Animals)
            {
                if (!animal.IsAlive.CurrentValue)
                {
                    continue;
                }

                aliveCount += 1;
            }

            if (aliveCount < _poolingSettings.MaxAliveAnimals)
            {

                return false;
            }

            return true;
        }
    }
}