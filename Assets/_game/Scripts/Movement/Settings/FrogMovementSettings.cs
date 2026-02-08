using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "FrogMovementSettings", menuName = "ZooWorld/Movement/FrogMovementSettings")]
    public sealed class FrogMovementSettings : AnimalMovementSettings
    {
        [field: SerializeField]
        [BoxGroup("Movement")]
        public float JumpDistance { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public float JumpInterval { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public float JumpDuration { get; private set; }

        [field: SerializeField]
        [BoxGroup("Movement")]
        public float JumpArcHeight { get; private set; }

        [field: SerializeField]
        [BoxGroup("Timing")]
        public float LandingFreezeSeconds { get; private set; } = 0.35f;

        [field: SerializeField]
        [BoxGroup("Timing")]
        public float RotationDurationSeconds { get; private set; } = 0.15f;

        [field: SerializeField]
        [BoxGroup("Ground")]
        public float GroundStickVelocityEpsilon { get; private set; } = 0.15f;

        [field: SerializeField]
        [BoxGroup("Ground")]
        public float GroundRayDistance { get; private set; } = 0.25f;

        [field: SerializeField]
        [BoxGroup("Ground")]
        public LayerMask GroundMask { get; private set; } = ~0;

        [field: SerializeField]
        [BoxGroup("Ground")]
        public bool UseGroundRayCheck { get; private set; } = true;

        [field: SerializeField]
        [BoxGroup("Forces")]
        public float ExtraDownForce { get; private set; } = 15f;
    }
}