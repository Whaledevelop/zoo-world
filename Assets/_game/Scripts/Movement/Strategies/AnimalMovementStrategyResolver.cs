using System;
using System.Collections.Generic;
using ZooWorld.Models;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public sealed class AnimalMovementStrategyResolver : IAnimalMovementStrategyResolver
    {
        private readonly IReadOnlyDictionary<Type, IAnimalMovementStrategy> _strategiesBySettingsType;
        private readonly NullAnimalMovementStrategy _fallback;

        public AnimalMovementStrategyResolver(
            FrogJumpMovementStrategy frog,
            SnakeLinearMovementStrategy snake,
            NullAnimalMovementStrategy fallback)
        {
            _fallback = fallback;
            _strategiesBySettingsType = new Dictionary<Type, IAnimalMovementStrategy>
            {
                { typeof(FrogMovementSettings), frog },
                { typeof(SnakeMovementSettings), snake }
            };
        }

        public IAnimalMovementStrategy Resolve(IAnimalModel animal)
        {
            var settingsType = animal.MovementSettings.GetType();

            var strategy = _strategiesBySettingsType.GetValueOrDefault(settingsType, _fallback);

            return strategy;
        }
    }
}