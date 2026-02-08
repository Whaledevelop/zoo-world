using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "PoolingSettings", menuName = "ZooWorld/Settings/Pooling")]
    public sealed class PoolingSettings : ScriptableObject
    {
        [field: SerializeField]
        [BoxGroup("Pooling")]
        public int PrewarmPerAnimal { get; private set; } = 8;

        [field: SerializeField]
        [BoxGroup("Pooling")]
        public int MaxSizePerAnimal { get; private set; } = 64;

        [field: SerializeField]
        [BoxGroup("Pooling")]
        public int MaxAliveAnimals { get; private set; } = 64;
    }
}