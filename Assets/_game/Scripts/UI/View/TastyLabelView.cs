using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ZooWorld.Views
{
    public sealed class TastyLabelView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _root;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TMP_Text _label;

        private Tween _tween;

        public void Play(Vector3 screenPosition, float duration, float riseDistance)
        {
            _tween?.Kill();

            _root.position = screenPosition;
            _root.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;
            _label.enabled = true;

            var targetPosition = _root.position + new Vector3(0f, riseDistance, 0f);

            _tween = DOTween.Sequence()
                .Join(_root.DOMove(targetPosition, duration))
                .Join(_root.DOScale(1.1f, duration * 0.5f))
                .Join(_canvasGroup.DOFade(0f, duration))
                .OnComplete(() =>
                {
                    _label.enabled = false;
                });
        }

        public void ResetLabel()
        {
            _label.enabled = true;
        }
    }
}