using System;
using System.Collections.Generic;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public sealed class CatalogAnimalMovementStrategyResolver : IAnimalMovementStrategyResolver
    {
        private readonly IReadOnlyDictionary<Type, AnimalMovementStrategyAsset> _strategiesBySettingsType;
        private readonly NullAnimalMovementStrategyAsset _fallback;

        public CatalogAnimalMovementStrategyResolver(
            AnimalMovementStrategyCatalog catalog,
            NullAnimalMovementStrategyAsset fallback)
        {
            _fallback = fallback;

            var strategiesBySettingsType = new Dictionary<Type, AnimalMovementStrategyAsset>();

            foreach (var binding in catalog.Bindings)
            {
                var settingsType = binding.SettingsPrototype.GetType();
                strategiesBySettingsType[settingsType] = binding.Strategy;
            }

            _strategiesBySettingsType = strategiesBySettingsType;
        }

        public AnimalMovementStrategyAsset Resolve(IAnimalModel animal)
        {
            var settingsType = animal.MovementSettings.GetType();
            var strategy = _strategiesBySettingsType.GetValueOrDefault(settingsType, _fallback);

            return strategy;
        }
    }
}