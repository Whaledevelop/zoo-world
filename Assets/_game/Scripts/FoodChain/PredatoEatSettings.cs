using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "PredatorEatSettings", menuName = "ZooWorld/Settings/Predator Eat")]
    public sealed class PredatorEatSettings : ScriptableObject
    {
        [field: SerializeField]
        public float TastyLabelDuration { get; private set; }

        [field: SerializeField]
        public Vector3 TastyLabelWorldOffset { get; private set; }

        [field: SerializeField]
        public float TastyLabelRiseDistance { get; private set; }
    }
}