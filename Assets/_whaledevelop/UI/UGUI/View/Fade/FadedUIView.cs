using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Utility;

namespace Whaledevelop.UI
{
    public abstract class FadedUIView<T> : UIView<T>, IAsyncInitializable, IAsyncReleasable where T : IUIViewModel
    {
        [SerializeField]
        [BoxGroup("Fade")]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        [BoxGroup("Fade")]
        private ViewFadeSettings _fadeSettings;

        private Tween _fadeTween;

        public bool Initialized { get; private set; }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            _canvasGroup.alpha = 0f;

            _fadeTween?.Kill();

            _fadeTween = _canvasGroup
                .DOFade(1f, _fadeSettings.FadeInDuration)
                .SetEase(_fadeSettings.FadeInEase);

            await _fadeTween.AwaitComplete(cancellationToken);

            Initialized = true;

            return;
        }

        public async UniTask ReleaseAsync(CancellationToken cancellationToken)
        {
            _fadeTween?.Kill();

            _fadeTween = _canvasGroup
                .DOFade(0f, _fadeSettings.FadeOutDuration)
                .SetEase(_fadeSettings.FadeOutEase);

            await _fadeTween.AwaitComplete(cancellationToken);

            Initialized = false;

            return;
        }
    }
}