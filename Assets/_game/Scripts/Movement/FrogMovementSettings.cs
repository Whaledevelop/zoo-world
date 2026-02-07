using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "FrogMovementSettings", menuName = "ZooWorld/Settings/Frog Movement")]
    public sealed class FrogMovementSettings : AnimalMovementSettings
    {
        [field: SerializeField]
        public float JumpDistance { get; private set; }

        [field: SerializeField]
        public float JumpInterval { get; private set; }

        [field: SerializeField]
        public float JumpDuration { get; private set; }

        [field: SerializeField]
        public float JumpArcHeight { get; private set; }
    }
}