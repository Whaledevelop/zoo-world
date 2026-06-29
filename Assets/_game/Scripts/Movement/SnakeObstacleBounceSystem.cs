﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using ZooWorld.Events;
using ZooWorld.Models;
using ZooWorld.Movement.Strategies;
using ZooWorld.Settings;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class SnakeObstacleBounceSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly IAnimalMovementStrategyResolver _strategyResolver;
        private readonly List<IDisposable> _subscriptions;

        public SnakeObstacleBounceSystem(IAnimalsModel animalsModel, IAnimalMovementStrategyResolver strategyResolver)
        {
            _animalsModel = animalsModel;
            _strategyResolver = strategyResolver;
            _subscriptions = new List<IDisposable>();
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _subscriptions.Add(_animalsModel.OnAnimalObstacleCollision.Subscribe(OnObstacleCollision));

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

        private void OnObstacleCollision(AnimalObstacleCollisionEvent collisionEvent)
        {
            var animal = collisionEvent.Animal;

            if (!animal.IsAlive.CurrentValue)
            {

                return;
            }

            if (animal.MovementSettings is not SnakeMovementSettings snakeMovementSettings)
            {

                return;
            }

            var strategy = _strategyResolver.Resolve(animal) as SnakeLinearMovementStrategyAsset;

            if (strategy == null)
            {

                return;
            }

            var collisionNormal = collisionEvent.Normal;
            var bounceDirection = new Vector3(collisionNormal.x, 0f, collisionNormal.z);

            if (bounceDirection.sqrMagnitude <= 0.0001f)
            {
                if (!strategy.TryGetDirection(animal.Id, out var currentDirection))
                {

                    return;
                }

                bounceDirection = -currentDirection;
            }

            bounceDirection = bounceDirection.normalized;
            strategy.SetDirection(animal.Id, bounceDirection, Time.time + snakeMovementSettings.TurnInterval);
        }
    }
}
