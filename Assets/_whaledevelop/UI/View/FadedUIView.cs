using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.UI
{
    public abstract class FadedUIView<T> : UIView<T> where T : IUIViewModel
    {
        [SerializeField]
        [BoxGroup("Fade")]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        [BoxGroup("Fade")]
        private ViewFadeSettings _fadeSettings;

        private Tween _fadeTween;

        public override void Initialize()
        {
            base.Initialize();

            _canvasGroup.alpha = 0f;
            Show();
        }

        public override void Release()
        {
            base.Release();
            Hide();
        }

        public void Show()
        {
            _fadeTween?.Kill();

            _fadeTween = _canvasGroup
                .DOFade(1f, _fadeSettings.FadeInDuration)
                .SetEase(_fadeSettings.FadeInEase);
        }

        public void Hide()
        {
            _fadeTween?.Kill();

            _fadeTween = _canvasGroup
                .DOFade(0f, _fadeSettings.FadeOutDuration)
                .SetEase(_fadeSettings.FadeOutEase);
        }
    }
}