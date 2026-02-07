using Sirenix.OdinInspector;
using UnityEngine;
using ZooWorld.Enums;
using ZooWorld.Views;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "AnimalSettings", menuName = "ZooWorld/Settings/AnimalSettings")]
    public sealed class AnimalSettings : ScriptableObject
    {
        [field: SerializeField]
        [BoxGroup("Identity")]
        public AnimalType Type { get; private set; }

        [field: SerializeField]
        [BoxGroup("Identity")]
        public AnimalGroup Group { get; private set; }

        [field: SerializeField]
        [BoxGroup("Prefab")]
        public AnimalView Prefab { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public AnimalMovementSettings AnimalMovementSettings { get; private set; }
    }
}