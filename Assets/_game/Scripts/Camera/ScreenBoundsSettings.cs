using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "ScreenBoundsSettings", menuName = "ZooWorld/Settings/Screen Bounds")]
    public sealed class ScreenBoundsSettings : ScriptableObject
    {
        [field: SerializeField]
        [Range(0f, 0.5f)]
        public float ViewportPadding { get; private set; } = 0.05f;

        [field: SerializeField]
        [Range(0f, 1f)]
        public float ReturnSteerStrength { get; private set; } = 0.35f;

        [field: SerializeField]
        [Range(0f, 1f)]
        public float FrogReturnBias { get; private set; } = 0.6f;

        [field: SerializeField]
        public bool UseViewportBounds { get; private set; } = true;
    }
}