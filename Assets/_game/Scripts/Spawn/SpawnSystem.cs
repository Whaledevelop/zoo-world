using System.Threading;
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
        private readonly ZooWorldRootView _worldRootView;
        private int _nextId;

        public SpawnSystem(
            ZooWorldSettings zooWorldSettings,
            AnimalSettingsTable animalSettingsTable,
            IAnimalsModel animalsModel,
            ZooWorldRootView worldRootView)
        {
            _zooWorldSettings = zooWorldSettings;
            _animalSettingsTable = animalSettingsTable;
            _animalsModel = animalsModel;
            _worldRootView = worldRootView;
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
            var definition = _animalSettingsTable.Definitions[Random.Range(0, _animalSettingsTable.Definitions.Count)];
            var spawnPosition = GetRandomPosition(_zooWorldSettings.WorldBounds);
            var model = new AnimalModel(++_nextId, definition, spawnPosition);
            var view = Object.Instantiate(definition.Prefab, spawnPosition, Quaternion.identity, _worldRootView.Root);

            _animalsModel.AddAnimal(model);
            view.Initialize(model, _animalsModel);
        }

        private Vector3 GetRandomPosition(Bounds bounds)
        {
            var xPosition = Random.Range(bounds.min.x, bounds.max.x);
            var zPosition = Random.Range(bounds.min.z, bounds.max.z);

            
            return new Vector3(xPosition, bounds.center.y, zPosition);
        }
    }
}