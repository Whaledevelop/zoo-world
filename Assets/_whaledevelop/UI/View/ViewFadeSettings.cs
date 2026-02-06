using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.UI
{
    [CreateAssetMenu(fileName = "ViewFadeSettings", menuName = "Whaledevelop/UI/View/ViewFadeSettings")]
    public class ViewFadeSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: BoxGroup("Fade in")]
        public float FadeInDuration { get; private set; } = 0.1f;

        [field: SerializeField]
        [field: BoxGroup("Fade in")]
        public Ease FadeInEase { get; private set; } = Ease.Linear;
        
        [field: SerializeField]
        [field: BoxGroup("Fade out")]
        public float FadeOutDuration { get; private set; } = 0.1f;

        [field: SerializeField]
        [field: BoxGroup("Fade out")]
        public Ease FadeOutEase { get; private set; } = Ease.Linear;
    }
}