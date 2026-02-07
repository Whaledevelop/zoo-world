using UnityEngine;

namespace ZooWorld.Settings
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "ZooWorld/Settings/UI")]
    public sealed class UISettings : ScriptableObject
    {
        [field: SerializeField]
        public string PreyCounterFormat { get; private set; }

        [field: SerializeField]
        public string PredatorCounterFormat { get; private set; }

        [field: SerializeField]
        public float CounterTweenDuration { get; private set; }

        [field: SerializeField]
        public float CounterPunchScale { get; private set; }
    }
}