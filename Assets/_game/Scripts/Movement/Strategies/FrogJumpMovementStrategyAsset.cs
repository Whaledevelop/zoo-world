using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public sealed class FrogJumpMovementStrategyAsset : AnimalMovementStrategyAsset
    {
        private readonly Dictionary<int, float> _nextJumpTimeById = new();

        public override void Tick(IAnimalModel animal, Rigidbody rigidbody, in AnimalMovementContext context)
        {
            var settings = (FrogMovementSettings)animal.MovementSettings;
            var time = context.Time;
            var nextJumpTime = _nextJumpTimeById.GetValueOrDefault(animal.Id, time);

            if (time < nextJumpTime)
            {

                return;
            }

            var randomDirection = Random.insideUnitCircle.normalized;
            var direction = new Vector3(randomDirection.x, 0f, randomDirection.y);
            direction = GetBiasedDirection(rigidbody.position, direction, context);

            var duration = Mathf.Max(settings.JumpDuration, 0.01f);
            var horizontalSpeed = settings.JumpDistance / duration;
            var horizontalVelocity = direction * horizontalSpeed;
            var jumpUpVelocity = 2f * settings.JumpArcHeight / duration;
            var velocity = new Vector3(horizontalVelocity.x, jumpUpVelocity, horizontalVelocity.z);

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.linearVelocity = velocity;
            _nextJumpTimeById[animal.Id] = time + settings.JumpInterval;
        }

        public override void Cleanup(int animalId)
        {
            _nextJumpTimeById.Remove(animalId);
        }

        private Vector3 GetBiasedDirection(Vector3 position, Vector3 direction, in AnimalMovementContext context)
        {
            var screenBoundsSettings = context.ScreenBoundsSettings;

            if (!screenBoundsSettings.UseViewportBounds || context.ViewportBoundsService.IsInside(position))
            {

                return direction;
            }

            var returnDirection = context.ViewportBoundsService.GetReturnDirection(position);
            var biasedDirection = Vector3.Lerp(direction, returnDirection, screenBoundsSettings.FrogReturnBias);

            return biasedDirection.normalized;
        }
    }
}