using System;
using System.Collections.Generic;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public sealed class CatalogAnimalMovementStrategyResolver : IAnimalMovementStrategyResolver
    {
        private readonly IReadOnlyDictionary<Type, AnimalMovementStrategyAsset> _strategiesBySettingsType;

        public CatalogAnimalMovementStrategyResolver(AnimalMovementStrategyCatalog catalog)
        {
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
            return _strategiesBySettingsType.GetValueOrDefault(settingsType);
        }
    }
}