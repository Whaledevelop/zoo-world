﻿using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Enums;
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
        private readonly AnimalSettingsTable _animalSettingsTable;
        private readonly IAnimalViewsRegistry _viewsRegistry;
        private readonly IViewportBoundsService _viewportBoundsService;
        private readonly ScreenBoundsSettings _screenBoundsSettings;
        private readonly Dictionary<int, float> _nextJumpTimeById;
        private readonly Dictionary<int, Vector3> _snakeDirectionById;
        private readonly Dictionary<int, float> _snakeTurnTimeById;

        public MovementSystem(
            IAnimalsModel animalsModel,
            AnimalSettingsTable animalSettingsTable,
            IAnimalViewsRegistry viewsRegistry,
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings)
        {
            _animalsModel = animalsModel;
            _animalSettingsTable = animalSettingsTable;
            _viewsRegistry = viewsRegistry;
            _viewportBoundsService = viewportBoundsService;
            _screenBoundsSettings = screenBoundsSettings;
            _nextJumpTimeById = new Dictionary<int, float>();
            _snakeDirectionById = new Dictionary<int, Vector3>();
            _snakeTurnTimeById = new Dictionary<int, float>();
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
                    CleanupAnimal(animal);

                    continue;
                }

                if (!_viewsRegistry.TryGetRigidbody(animal.Id, out var rigidbody))
                {

                    continue;
                }

                switch (animal.Type)
                {
                    case AnimalType.Frog when animal.MovementSettings is FrogMovementSettings frogMovementSettings:
                        UpdateFrog(animal, rigidbody, frogMovementSettings);
                        break;
                    case AnimalType.Snake when animal.MovementSettings is SnakeMovementSettings snakeMovementSettings:
                        UpdateSnake(animal, rigidbody, snakeMovementSettings);
                        break;
                }
            }
        }

        private void UpdateFrog(IAnimalModel animal, Rigidbody rigidbody, FrogMovementSettings settings)
        {
            var time = Time.time;

            var nextJumpTime = _nextJumpTimeById.GetValueOrDefault(animal.Id, time);

            if (time < nextJumpTime)
            {

                return;
            }

            var randomDirection = Random.insideUnitCircle.normalized;
            var direction = new Vector3(randomDirection.x, 0f, randomDirection.y);
            direction = GetBiasedDirection(rigidbody.position, direction, _screenBoundsSettings.FrogReturnBias);

            var duration = Mathf.Max(settings.JumpDuration, 0.01f);
            var horizontalSpeed = settings.JumpDistance / duration;
            var horizontalVelocity = direction * horizontalSpeed;
            var jumpUpVelocity = 2f * settings.JumpArcHeight / duration;
            var velocity = new Vector3(horizontalVelocity.x, jumpUpVelocity, horizontalVelocity.z);

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.linearVelocity = velocity;
            _nextJumpTimeById[animal.Id] = time + settings.JumpInterval;
        }

        private void UpdateSnake(IAnimalModel animal, Rigidbody rigidbody, SnakeMovementSettings settings)
        {
            var time = Time.time;

            if (!_snakeDirectionById.TryGetValue(animal.Id, out var direction))
            {
                direction = GetRandomDirection();
                _snakeDirectionById[animal.Id] = direction;
                _snakeTurnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            if (_snakeTurnTimeById.TryGetValue(animal.Id, out var nextTurnTime) && time >= nextTurnTime)
            {
                direction = Quaternion.Euler(0f, Random.Range(-settings.TurnAngleRange, settings.TurnAngleRange), 0f) * direction;
                direction = direction.normalized;
                _snakeDirectionById[animal.Id] = direction;
                _snakeTurnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            direction = GetReturnSteerDirection(rigidbody.position, direction);
            _snakeDirectionById[animal.Id] = direction;

            var targetVelocity = direction * settings.Speed;
            var rigidbodyVelocity = rigidbody.linearVelocity;
            rigidbodyVelocity.x = targetVelocity.x;
            rigidbodyVelocity.z = targetVelocity.z;
            rigidbody.linearVelocity = rigidbodyVelocity;
        }

        private Vector3 GetRandomDirection()
        {
            var randomDirection = Random.insideUnitCircle.normalized;

            return new Vector3(randomDirection.x, 0f, randomDirection.y);
        }

        private Vector3 GetReturnSteerDirection(Vector3 position, Vector3 direction)
        {
            if (!_screenBoundsSettings.UseViewportBounds || _viewportBoundsService.IsInside(position))
            {

                return direction;
            }

            var returnDirection = _viewportBoundsService.GetReturnDirection(position);
            var steeredDirection = Vector3.Lerp(direction, returnDirection, _screenBoundsSettings.ReturnSteerStrength);

            return steeredDirection.normalized;
        }

        private Vector3 GetBiasedDirection(Vector3 position, Vector3 direction, float bias)
        {
            if (!_screenBoundsSettings.UseViewportBounds || _viewportBoundsService.IsInside(position))
            {

                return direction;
            }

            var returnDirection = _viewportBoundsService.GetReturnDirection(position);
            var biasedDirection = Vector3.Lerp(direction, returnDirection, bias);

            return biasedDirection.normalized;
        }
        
        private void CleanupAnimal(IAnimalModel animal)
        {
            _nextJumpTimeById.Remove(animal.Id);
            _snakeDirectionById.Remove(animal.Id);
            _snakeTurnTimeById.Remove(animal.Id);
        }
    }
}