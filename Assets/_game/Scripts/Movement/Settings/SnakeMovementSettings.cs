using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "SnakeMovementSettings", menuName = "ZooWorld/Movement/SnakeMovementSettings")]
    public sealed class SnakeMovementSettings : AnimalMovementSettings
    {
        [field: SerializeField]
        [BoxGroup("Movement")]
        public float Speed { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public float TurnInterval { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public float TurnAngleRange { get; private set; }

        [field: SerializeField]
        [BoxGroup("Rotation")]
        public float TurnSpeedDegPerSec { get; private set; } = 720f;

        [field: SerializeField]
        [BoxGroup("Rotation")]
        public bool SnapIfVeryClose { get; private set; } = true;

        [field: SerializeField]
        [BoxGroup("Rotation")]
        public float SnapAngleThresholdDeg { get; private set; } = 2f;
    }
}