using Sirenix.OdinInspector;
using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "ZooWorldSettings", menuName = "ZooWorld/Settings/Zoo World")]
    public sealed class ZooWorldSettings : ScriptableObject
    {
        [field: SerializeField]
        [BoxGroup("Spawning")]
        public Vector2 SpawnIntervalRange { get; private set; }

        [field: SerializeField]
        [BoxGroup("World")]
        public Bounds WorldBounds { get; private set; }

        [field: SerializeField]
        [BoxGroup("Camera")]
        public Vector3 CameraPosition { get; private set; }

        [field: SerializeField]
        [BoxGroup("Camera")]
        public Vector3 CameraRotation { get; private set; }

        [field: SerializeField]
        [BoxGroup("Camera")]
        public float CameraOrthographicSize { get; private set; }
    }
}