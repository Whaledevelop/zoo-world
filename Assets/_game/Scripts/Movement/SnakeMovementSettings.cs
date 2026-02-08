using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "SnakeMovementSettings", menuName = "ZooWorld/Movement/SnakeMovementSettings")]
    public sealed class SnakeMovementSettings : AnimalMovementSettings
    {
        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField]
        public float TurnInterval { get; private set; }

        [field: SerializeField]
        public float TurnAngleRange { get; private set; }
    }
}