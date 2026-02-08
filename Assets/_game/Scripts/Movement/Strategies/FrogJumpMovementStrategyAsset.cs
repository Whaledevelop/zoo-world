using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Movement.Strategies
{
    [CreateAssetMenu(fileName = "FrogJumpMovementStrategyAsset", menuName = "ZooWorld/Movement/FrogJumpMovementStrategyAsset")]
    public sealed class FrogJumpMovementStrategyAsset : AnimalMovementStrategyAsset
    {
        private readonly Dictionary<int, FrogState> _stateById = new();
        private readonly Dictionary<int, float> _nextActionTimeById = new();
        private readonly Dictionary<int, Vector3> _nextJumpDirectionById = new();
        private readonly Dictionary<int, Quaternion> _targetRotationById = new();
        private readonly Dictionary<int, float> _defaultLinearDampingById = new();
        private readonly Dictionary<int, float> _defaultAngularDampingById = new();
        private readonly Dictionary<int, RigidbodyConstraints> _defaultConstraintsById = new();

        public override void Tick(IAnimalModel animal, Rigidbody rigidbody, in AnimalMovementContext context)
        {
            var movementSettings = (FrogMovementSettings)animal.MovementSettings;
            var time = context.Time;

            if (!context.ViewsRegistry.TryGet(animal.Id, out var view))
            {

                return;
            }

            if (!_stateById.TryGetValue(animal.Id, out var state))
            {
                state = FrogState.Waiting;
                _stateById[animal.Id] = state;
            }

            if (state == FrogState.Waiting)
            {
                TickWaiting(animal.Id, rigidbody, view, movementSettings, context, time);

                return;
            }

            if (state == FrogState.Rotating)
            {
                TickRotating(animal.Id, rigidbody, view, movementSettings, context, time);

                return;
            }

            TickJumping(animal.Id, rigidbody, movementSettings, time);
        }

        public override void Cleanup(int animalId)
        {
            _stateById.Remove(animalId);
            _nextActionTimeById.Remove(animalId);
            _nextJumpDirectionById.Remove(animalId);
            _targetRotationById.Remove(animalId);
            
            _defaultLinearDampingById.Remove(animalId);
            _defaultAngularDampingById.Remove(animalId);
            _defaultConstraintsById.Remove(animalId);
        }

        private void TickWaiting(
            int animalId,
            Rigidbody rigidbody,
            AnimalView view,
            FrogMovementSettings movementSettings,
            in AnimalMovementContext context,
            float time)
        {
            var nextActionTime = GetNextActionTime(animalId, time);
            var currentVelocity = rigidbody.linearVelocity;
            rigidbody.linearVelocity = new Vector3(0f, currentVelocity.y, 0f);

            if (time < nextActionTime)
            {

                return;
            }

            if (!TryGetJumpDirection(rigidbody.position, movementSettings, context, out var direction))
            {

                return;
            }

            RestoreMovementState(animalId, rigidbody);
            _nextJumpDirectionById[animalId] = direction;
            _targetRotationById[animalId] = GetTargetRotation(view, direction);
            _nextActionTimeById[animalId] = time + movementSettings.RotationDurationSeconds;
            _stateById[animalId] = FrogState.Rotating;
        }

        private void TickRotating(
            int animalId,
            Rigidbody rigidbody,
            AnimalView view,
            FrogMovementSettings movementSettings,
            in AnimalMovementContext context,
            float time)
        {
            var nextActionTime = GetNextActionTime(animalId, time);

            if (!_targetRotationById.TryGetValue(animalId, out var targetRotation) ||
                !_nextJumpDirectionById.TryGetValue(animalId, out var direction))
            {
                _stateById[animalId] = FrogState.Waiting;
                _nextActionTimeById[animalId] = time;

                return;
            }

            var rotationDuration = Mathf.Max(movementSettings.RotationDurationSeconds, 0.01f);
            var currentRotation = view.transform.rotation;
            var angle = Quaternion.Angle(currentRotation, targetRotation);
            var rotationSpeed = angle / rotationDuration;
            var maxStep = rotationSpeed * context.DeltaTime;

            view.transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxStep);

            if (time < nextActionTime)
            {

                return;
            }

            ApplyJumpVelocity(rigidbody, movementSettings, direction);
            _stateById[animalId] = FrogState.Jumping;
        }

        private void TickJumping(int animalId, Rigidbody rigidbody, FrogMovementSettings movementSettings, float time)
        {
            if (movementSettings.ExtraDownForce > 0f && rigidbody.linearVelocity.y < 0f)
            {
                rigidbody.AddForce(Vector3.down * movementSettings.ExtraDownForce, ForceMode.Acceleration);
            }

            if (!IsLanded(rigidbody, movementSettings))
            {

                return;
            }

            ApplyLandingState(rigidbody, movementSettings);
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.Sleep();
            _stateById[animalId] = FrogState.Waiting;
            _nextActionTimeById[animalId] = time + movementSettings.LandingFreezeSeconds;
        }

        private float GetNextActionTime(int animalId, float time)
        {
            if (_nextActionTimeById.TryGetValue(animalId, out var nextActionTime))
            {

                return nextActionTime;
            }

            _nextActionTimeById[animalId] = time;

            return time;
        }
        
        private void EnsureDefaults(int animalId, Rigidbody rigidbody)
        {
            if (_defaultConstraintsById.ContainsKey(animalId))
            {

                return;
            }

            _defaultLinearDampingById[animalId] = rigidbody.linearDamping;
            _defaultAngularDampingById[animalId] = rigidbody.angularDamping;
            _defaultConstraintsById[animalId] = rigidbody.constraints;
        }

        private void ApplyLandingState(Rigidbody rigidbody, FrogMovementSettings movementSettings)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.linearDamping = movementSettings.LandingLinearDamping;
            rigidbody.angularDamping = movementSettings.LandingAngularDamping;

            if (!movementSettings.FreezeXZOnLanding)
            {

                return;
            }

            rigidbody.constraints = rigidbody.constraints
                                    | RigidbodyConstraints.FreezePositionX
                                    | RigidbodyConstraints.FreezePositionZ
                                    | RigidbodyConstraints.FreezeRotationX
                                    | RigidbodyConstraints.FreezeRotationZ;
        }

        private void RestoreMovementState(int animalId, Rigidbody rigidbody)
        {
            if (_defaultLinearDampingById.TryGetValue(animalId, out var linearDamping))
            {
                rigidbody.linearDamping = linearDamping;
            }

            if (_defaultAngularDampingById.TryGetValue(animalId, out var angularDamping))
            {
                rigidbody.angularDamping = angularDamping;
            }

            if (_defaultConstraintsById.TryGetValue(animalId, out var constraints))
            {
                rigidbody.constraints = constraints;
            }
        }

        private void ApplyJumpVelocity(Rigidbody rigidbody, FrogMovementSettings settings, Vector3 direction)
        {
            var duration = Mathf.Max(settings.JumpDuration, 0.01f);
            var horizontalSpeed = settings.JumpDistance / duration;
            var horizontalVelocity = direction * horizontalSpeed;
            var jumpUpVelocity = 2f * settings.JumpArcHeight / duration;
            var velocity = new Vector3(horizontalVelocity.x, jumpUpVelocity, horizontalVelocity.z);

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.linearVelocity = velocity;
        }

        private bool IsLanded(Rigidbody rigidbody, FrogMovementSettings movementSettings)
        {
            var verticalVelocity = rigidbody.linearVelocity.y;

            if (movementSettings.UseGroundRayCheck)
            {
                if (verticalVelocity > movementSettings.GroundStickVelocityEpsilon)
                {

                    return false;
                }

                if (Physics.Raycast(
                        rigidbody.position,
                        Vector3.down,
                        movementSettings.GroundRayDistance,
                        movementSettings.GroundMask,
                        QueryTriggerInteraction.Ignore))
                {

                    return true;
                }

                return false;
            }

            if (Mathf.Abs(verticalVelocity) <= movementSettings.GroundStickVelocityEpsilon)
            {

                return true;
            }

            return false;
        }

        private bool TryGetJumpDirection(
            Vector3 position,
            FrogMovementSettings settings,
            in AnimalMovementContext context,
            out Vector3 direction)
        {
            var obstacleSettings = context.ObstacleSettings;

            for (var attempt = 0; attempt < obstacleSettings.MaxDirectionAttempts; attempt++)
            {
                var randomDirection = Random.insideUnitCircle.normalized;
                var candidateDirection = new Vector3(randomDirection.x, 0f, randomDirection.y);
                candidateDirection = GetBiasedDirection(position, candidateDirection, context);

                if (!obstacleSettings.UseObstacleChecks)
                {
                    direction = candidateDirection;

                    return true;
                }

                var endpoint = position + candidateDirection * settings.JumpDistance;

                if (context.ObstacleQueryService.IsBlocked(endpoint, obstacleSettings.MovementCheckRadius))
                {
                    continue;
                }

                direction = candidateDirection;

                return true;
            }

            direction = Vector3.zero;

            return false;
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
        
        private Quaternion GetTargetRotation(AnimalView view, Vector3 direction)
        {
            var flattenedDirection = new Vector3(direction.x, 0f, direction.z);
            var targetYaw = Quaternion.LookRotation(flattenedDirection, Vector3.up);
            var initialYaw = Quaternion.LookRotation(view.InitialForwardDirection, Vector3.up);

            return targetYaw * Quaternion.Inverse(initialYaw) * view.InitialRotation;
        }

        private enum FrogState
        {
            Waiting,
            Rotating,
            Jumping
        }
    }
}