using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Services;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public sealed class FrogJumpMovementStrategy : IAnimalMovementStrategy
    {
        private readonly IViewportBoundsService _viewportBoundsService;
        private readonly ScreenBoundsSettings _screenBoundsSettings;
        private readonly Dictionary<int, float> _nextJumpTimeById;

        public FrogJumpMovementStrategy(
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings)
        {
            _viewportBoundsService = viewportBoundsService;
            _screenBoundsSettings = screenBoundsSettings;
            _nextJumpTimeById = new Dictionary<int, float>();
        }

        public void Tick(IAnimalModel animal, Rigidbody rigidbody, float time)
        {
            var settings = (FrogMovementSettings)animal.MovementSettings;
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

        public void Cleanup(int animalId)
        {
            _nextJumpTimeById.Remove(animalId);
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
    }
}