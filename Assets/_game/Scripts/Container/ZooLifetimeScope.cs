﻿using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Whaledevelop.VContainer;
using ZooWorld.Models;
using ZooWorld.Movement.Strategies;
using ZooWorld.Obstacles;
using ZooWorld.Services;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Systems
{
    public sealed class ZooLifetimeScope : GameLifetimeScopeBase<ZooEntryPoint>
    {
        [SerializeField]
        [BoxGroup("Settings")]
        private ZooWorldSettings _zooWorldSettings;

        [SerializeField]
        [BoxGroup("Settings")]
        private AnimalSettingsTable _animalSettingsTable;

        [SerializeField]
        [BoxGroup("Settings")]
        private PredatorEatSettings _predatorEatSettings;

        [SerializeField]
        [BoxGroup("Settings")]
        private UISettings _uiSettings;

        [SerializeField]
        [BoxGroup("Views")]
        private ZooUIView _uiView;

        [SerializeField]
        [BoxGroup("Views")]
        private ZooWorldRootView _worldRootView;

        [SerializeField]
        [BoxGroup("Views")]
        private CameraView _cameraView;

        [SerializeField]
        [BoxGroup("Settings")]
        private ScreenBoundsSettings _screenBoundsSettings;

        [SerializeField]
        [BoxGroup("Settings")]
        private AnimalMovementStrategyCatalog _movementStrategyCatalog;

        [SerializeField]
        [BoxGroup("Settings")]
        private PoolingSettings _poolingSettings;

        [SerializeField]
        [BoxGroup("Settings")]
        private ObstacleSettings _obstacleSettings;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_zooWorldSettings);
            builder.RegisterInstance(_animalSettingsTable);
            builder.RegisterInstance(_predatorEatSettings);
            builder.RegisterInstance(_uiSettings);
            builder.RegisterInstance(_screenBoundsSettings);
            builder.RegisterInstance(_movementStrategyCatalog);
            builder.RegisterInstance(_poolingSettings);
            builder.RegisterInstance(_obstacleSettings);
            builder.RegisterInstance(_uiView);
            builder.RegisterInstance(_worldRootView);
            builder.RegisterInstance(_cameraView);

            builder.Register<AnimalsModel>(Lifetime.Singleton)
                .As<IAnimalsModel>();

            builder.Register<AnimalViewsRegistry>(Lifetime.Singleton)
                .As<IAnimalViewsRegistry>();
            
            builder.Register<AnimalViewsPool>(Lifetime.Singleton)
                .As<IAnimalViewsPool>();
            
            builder.Register<ICameraModel>(
                    container => new CameraModel(
                        _zooWorldSettings.WorldBounds,
                        _zooWorldSettings.CameraPosition,
                        _zooWorldSettings.CameraRotation,
                        _zooWorldSettings.CameraOrthographicSize),
                    Lifetime.Singleton)
                .As<ICameraModel>();

            builder.Register<ViewportBoundsService>(Lifetime.Singleton)
                .As<IViewportBoundsService>();

            builder.Register<ObstacleQueryService>(Lifetime.Singleton)
                .As<IObstacleQueryService>();

            builder.Register<CatalogAnimalMovementStrategyResolver>(Lifetime.Singleton)
                .As<IAnimalMovementStrategyResolver>();

            builder.Register<SpawnSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<MovementSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<BoundsSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<FoodChainSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<AnimalDespawnSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<UISystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();

            builder.Register<CameraSystem>(Lifetime.Singleton)
                .AsSelf()
                .As<IAsyncInitializable>()
                .As<IAsyncReleasable>();
        }
    }
}