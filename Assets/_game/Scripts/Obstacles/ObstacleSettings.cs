using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "ObstacleSettings", menuName = "ZooWorld/Settings/Obstacles")]
    public sealed class ObstacleSettings : ScriptableObject
    {
        [field: SerializeField]
        [BoxGroup("Checks")]
        public LayerMask ObstaclesMask { get; private set; }

        [field: SerializeField]
        [BoxGroup("Checks")]
        public float SpawnCheckRadius { get; private set; } = 0.6f;

        [field: SerializeField]
        [BoxGroup("Checks")]
        public float MovementCheckRadius { get; private set; } = 0.6f;

        [field: SerializeField]
        [BoxGroup("Checks")]
        public float SnakeLookAheadSeconds { get; private set; } = 0.6f;

        [field: SerializeField]
        [BoxGroup("Checks")]
        public bool UseObstacleChecks { get; private set; } = true;

        [field: SerializeField]
        [BoxGroup("Limits")]
        public int MaxSpawnAttempts { get; private set; } = 20;

        [field: SerializeField]
        [BoxGroup("Limits")]
        public int MaxDirectionAttempts { get; private set; } = 8;
    }
}