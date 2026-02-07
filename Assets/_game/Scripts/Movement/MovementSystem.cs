using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using ZooWorld.Enums;
using ZooWorld.Models;
using ZooWorld.Settings;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class MovementSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly AnimalSettingsTable _animalSettingsTable;
        private readonly Dictionary<int, float> _nextJumpTimeById;
        private readonly Dictionary<int, Tween> _frogJumpTweensById;
        private readonly Dictionary<int, Vector3> _snakeDirectionById;
        private readonly Dictionary<int, float> _snakeTurnTimeById;

        public MovementSystem(IAnimalsModel animalsModel, AnimalSettingsTable animalSettingsTable)
        {
            _animalsModel = animalsModel;
            _animalSettingsTable = animalSettingsTable;
            _nextJumpTimeById = new Dictionary<int, float>();
            _frogJumpTweensById = new Dictionary<int, Tween>();
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
                UpdateAnimals(Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        private void UpdateAnimals(float deltaTime)
        {
            foreach (var animal in _animalsModel.Animals)
            {
                switch (animal.Type)
                {
                    case AnimalType.Frog when animal.MovementSettings is FrogMovementSettings frogMovementSettings:
                        UpdateFrog(animal, frogMovementSettings);
                        break;
                    case AnimalType.Snake when animal.MovementSettings is SnakeMovementSettings snakeMovementSettings:
                        UpdateSnake(animal, snakeMovementSettings, deltaTime);
                        break;
                }
            }
        }

        private void UpdateFrog(IAnimalModel animal, FrogMovementSettings settings)
        {
            if (_frogJumpTweensById.TryGetValue(animal.Id, out var existingTween) && existingTween.IsActive())
            {

                return;
            }

            var time = Time.time;

            var nextJumpTime = _nextJumpTimeById.GetValueOrDefault(animal.Id, time);

            if (time < nextJumpTime)
            {

                return;
            }

            var startPosition = animal.Position.CurrentValue;
            var randomDirection = Random.insideUnitCircle.normalized;
            var direction = new Vector3(randomDirection.x, 0f, randomDirection.y);
            var targetPosition = startPosition + direction * settings.JumpDistance;
            var duration = Mathf.Max(settings.JumpDuration, 0.01f);
            var arcHeight = settings.JumpArcHeight;

            var tween = DOTween.To(
                    () => 0f,
                    progress =>
                    {
                        var heightOffset = Mathf.Sin(progress * Mathf.PI) * arcHeight;
                        var horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                        animal.SetPosition(horizontalPosition + Vector3.up * heightOffset);
                    },
                    1f,
                    duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _frogJumpTweensById.Remove(animal.Id);
                });

            _frogJumpTweensById[animal.Id] = tween;
            _nextJumpTimeById[animal.Id] = time + settings.JumpInterval;
        }

        private void UpdateSnake(IAnimalModel animal, SnakeMovementSettings settings, float deltaTime)
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

            var displacement = direction * (settings.Speed * deltaTime);
            var position = animal.Position.CurrentValue + displacement;
            animal.SetPosition(position);
            animal.SetVelocity(displacement / Mathf.Max(deltaTime, 0.01f));
        }

        private Vector3 GetRandomDirection()
        {
            var randomDirection = Random.insideUnitCircle.normalized;

            return new Vector3(randomDirection.x, 0f, randomDirection.y);
        }
    }
}