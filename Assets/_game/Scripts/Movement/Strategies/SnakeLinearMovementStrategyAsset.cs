using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Movement.Strategies
{
    [CreateAssetMenu(fileName = "SnakeLinearMovementStrategyAsset", menuName = "ZooWorld/Movement/SnakeLinearMovementStrategyAsset")]
    public sealed class SnakeLinearMovementStrategyAsset : AnimalMovementStrategyAsset
    {
        private readonly Dictionary<int, Vector3> _directionById = new();
        private readonly Dictionary<int, float> _turnTimeById = new();

        public override void Tick(IAnimalModel animal, Rigidbody rigidbody, in AnimalMovementContext context)
        {
            var settings = (SnakeMovementSettings)animal.MovementSettings;
            var time = context.Time;

            if (!_directionById.TryGetValue(animal.Id, out var direction))
            {
                if (!TryGetFreeDirection(rigidbody.position, settings.Speed, context, out direction))
                {

                    return;
                }

                _directionById[animal.Id] = direction;
                _turnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            if (_turnTimeById.TryGetValue(animal.Id, out var nextTurnTime) && time >= nextTurnTime)
            {
                if (TryGetTurnDirection(rigidbody.position, direction, settings, context, out var turnDirection))
                {
                    direction = turnDirection;
                    _directionById[animal.Id] = direction;
                    _turnTimeById[animal.Id] = time + settings.TurnInterval;
                }
            }

            direction = GetReturnSteerDirection(rigidbody.position, direction, context);

            if (context.ObstacleSettings.UseObstacleChecks &&
                IsDirectionBlocked(rigidbody.position, direction, settings.Speed, context))
            {
                if (!TryGetFreeDirection(rigidbody.position, settings.Speed, context, out direction))
                {
                    Stop(rigidbody);

                    return;
                }

                _turnTimeById[animal.Id] = time + settings.TurnInterval;
            }

            _directionById[animal.Id] = direction;

            var targetVelocity = direction * settings.Speed;
            var rigidbodyVelocity = rigidbody.linearVelocity;
            rigidbodyVelocity.x = targetVelocity.x;
            rigidbodyVelocity.z = targetVelocity.z;
            rigidbody.linearVelocity = rigidbodyVelocity;
            
            if (!context.ViewsRegistry.TryGet(animal.Id, out var view) || direction.sqrMagnitude <= 0f)
            {

                return;
            }

            RotateTowardsDirection(view, direction, settings, context.DeltaTime);
        }

        public bool TryGetDirection(int animalId, out Vector3 direction)
        {
            if (_directionById.TryGetValue(animalId, out var storedDirection))
            {
                direction = storedDirection;

                return true;
            }

            direction = Vector3.zero;

            return false;
        }

        public void SetDirection(int animalId, Vector3 direction, float nextTurnTime)
        {
            _directionById[animalId] = direction;
            _turnTimeById[animalId] = nextTurnTime;
        }
        
        public override void Cleanup(int animalId)
        {
            _directionById.Remove(animalId);
            _turnTimeById.Remove(animalId);
        }

        private bool TryGetFreeDirection(
            Vector3 position,
            float speed,
            in AnimalMovementContext context,
            out Vector3 direction)
        {
            var obstacleSettings = context.ObstacleSettings;
            var initialAngle = Random.Range(0f, 360f);
            var angleStep = 360f / obstacleSettings.MaxDirectionAttempts;

            for (var attempt = 0; attempt < obstacleSettings.MaxDirectionAttempts; attempt++)
            {
                var candidateAngle = initialAngle + angleStep * attempt;
                var candidateDirection = Quaternion.Euler(0f, candidateAngle, 0f) * Vector3.forward;

                if (!obstacleSettings.UseObstacleChecks)
                {
                    direction = candidateDirection;

                    return true;
                }

                if (IsDirectionBlocked(position, candidateDirection, speed, context))
                {
                    continue;
                }

                direction = candidateDirection;

                return true;
            }

            direction = Vector3.zero;

            return false;
        }

        private bool TryGetTurnDirection(
            Vector3 position,
            Vector3 currentDirection,
            SnakeMovementSettings settings,
            in AnimalMovementContext context,
            out Vector3 direction)
        {
            var obstacleSettings = context.ObstacleSettings;

            for (var attempt = 0; attempt < obstacleSettings.MaxDirectionAttempts; attempt++)
            {
                var candidateDirection = Quaternion.Euler(0f, Random.Range(-settings.TurnAngleRange, settings.TurnAngleRange), 0f) * currentDirection;
                candidateDirection = candidateDirection.normalized;

                if (!obstacleSettings.UseObstacleChecks)
                {
                    direction = candidateDirection;

                    return true;
                }

                if (IsDirectionBlocked(position, candidateDirection, settings.Speed, context))
                {
                    continue;
                }

                direction = candidateDirection;

                return true;
            }

            direction = currentDirection;

            return false;
        }

        private bool IsDirectionBlocked(
            Vector3 position,
            Vector3 direction,
            float speed,
            in AnimalMovementContext context)
        {
            var obstacleSettings = context.ObstacleSettings;
            var lookAheadDistance = speed * obstacleSettings.SnakeLookAheadSeconds;

            return context.ObstacleQueryService.IsPathBlocked(
                position,
                direction,
                lookAheadDistance,
                obstacleSettings.MovementCheckRadius);
        }

        private void Stop(Rigidbody rigidbody)
        {
            var velocity = rigidbody.linearVelocity;
            velocity.x = 0f;
            velocity.z = 0f;
            rigidbody.linearVelocity = velocity;
        }

        private Vector3 GetReturnSteerDirection(Vector3 position, Vector3 direction, in AnimalMovementContext context)
        {
            var screenBoundsSettings = context.ScreenBoundsSettings;

            if (!screenBoundsSettings.UseViewportBounds || context.ViewportBoundsService.IsInside(position))
            {

                return direction;
            }

            var returnDirection = context.ViewportBoundsService.GetReturnDirection(position);
            var steeredDirection = Vector3.Lerp(direction, returnDirection, screenBoundsSettings.ReturnSteerStrength);

            return steeredDirection.normalized;
        }
        
        private void RotateTowardsDirection(AnimalView view, Vector3 direction, SnakeMovementSettings settings, float deltaTime)
        {
            var flattenedDirection = new Vector3(direction.x, 0f, direction.z);
            var targetRotation = GetTargetRotation(view, flattenedDirection);
            var maxStep = settings.TurnSpeedDegPerSec * deltaTime;
            var currentRotation = view.transform.rotation;
            var angle = Quaternion.Angle(currentRotation, targetRotation);

            if (settings.SnapIfVeryClose && angle <= settings.SnapAngleThresholdDeg)
            {
                view.transform.rotation = targetRotation;

                return;
            }

            view.transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxStep);
        }

        private Quaternion GetTargetRotation(AnimalView view, Vector3 direction)
        {
            var targetYaw = Quaternion.LookRotation(direction, Vector3.up);
            var initialYaw = Quaternion.LookRotation(view.InitialForwardDirection, Vector3.up);

            return targetYaw * Quaternion.Inverse(initialYaw) * view.InitialRotation;
        }
    }
}
