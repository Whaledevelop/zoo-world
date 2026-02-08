﻿using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;

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

            direction = GetReturnSteerDirection(rigidbody.position, direction, context);
            _directionById[animal.Id] = direction;

            var targetVelocity = direction * settings.Speed;
            var rigidbodyVelocity = rigidbody.linearVelocity;
            rigidbodyVelocity.x = targetVelocity.x;
            rigidbodyVelocity.z = targetVelocity.z;
            rigidbody.linearVelocity = rigidbodyVelocity;
        }

        public override void Cleanup(int animalId)
        {
            _directionById.Remove(animalId);
            _turnTimeById.Remove(animalId);
        }

        private Vector3 GetRandomDirection()
        {
            var randomDirection = Random.insideUnitCircle.normalized;

            return new Vector3(randomDirection.x, 0f, randomDirection.y);
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
    }
}