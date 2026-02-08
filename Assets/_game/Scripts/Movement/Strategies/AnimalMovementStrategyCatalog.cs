using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    [CreateAssetMenu(fileName = "AnimalMovementStrategyCatalog", menuName = "ZooWorld/Settings/Movement Strategy Catalog")]
    public sealed class AnimalMovementStrategyCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class Binding
        {
            [field: SerializeField]
            public AnimalMovementSettings SettingsPrototype { get; private set; }

            [field: SerializeField]
            public AnimalMovementStrategyAsset Strategy { get; private set; }
        }

        [field: SerializeField]
        [BoxGroup("Settings")]
        public List<Binding> Bindings { get; private set; }
    }
}