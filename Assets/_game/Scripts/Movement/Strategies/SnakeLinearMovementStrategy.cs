using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Services;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public sealed class SnakeLinearMovementStrategy : IAnimalMovementStrategy
    {
        private readonly IViewportBoundsService _viewportBoundsService;
        private readonly ScreenBoundsSettings _screenBoundsSettings;
        private readonly Dictionary<int, Vector3> _directionById;
        private readonly Dictionary<int, float> _turnTimeById;

        public SnakeLinearMovementStrategy(
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings)
        {
            _viewportBoundsService = viewportBoundsService;
            _screenBoundsSettings = screenBoundsSettings;
            _directionById = new Dictionary<int, Vector3>();
            _turnTimeById = new Dictionary<int, float>();
        }

        public void Tick(IAnimalModel animal, Rigidbody rigidbody, float time)
        {
            var settings = (SnakeMovementSettings)animal.MovementSettings;

            if (!_directionById.TryGetValue(animal.Id, out var direction))
            {
                direction = GetRandomDirection();
                _directionById[animal.Id] = direction;
                _turnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            if (_turnTimeById.TryGetValue(animal.Id, out var nextTurnTime) && time >= nextTurnTime)
            {
                direction = Quaternion.Euler(0f, Random.Range(-settings.TurnAngleRange, settings.TurnAngleRange), 0f) * direction;
                direction = direction.normalized;
                _directionById[animal.Id] = direction;
                _turnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            direction = GetReturnSteerDirection(rigidbody.position, direction);
            _directionById[animal.Id] = direction;

            var targetVelocity = direction * settings.Speed;
            var rigidbodyVelocity = rigidbody.linearVelocity;
            rigidbodyVelocity.x = targetVelocity.x;
            rigidbodyVelocity.z = targetVelocity.z;
            rigidbody.linearVelocity = rigidbodyVelocity;
        }

        public void Cleanup(int animalId)
        {
            _directionById.Remove(animalId);
            _turnTimeById.Remove(animalId);
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
    }
}