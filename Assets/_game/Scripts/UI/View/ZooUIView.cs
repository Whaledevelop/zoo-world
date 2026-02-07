using TMPro;
using UnityEngine;
using DG.Tweening;

namespace ZooWorld.Views
{
    public sealed class ZooUIView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _preyCounter;

        [SerializeField]
        private TMP_Text _predatorCounter;

        [SerializeField]
        private TastyLabelView _tastyLabel;

        [SerializeField]
        private Camera _uiCamera;

        public Camera UICamera => _uiCamera;

        public void SetPreyCounter(string text)
        {
            _preyCounter.text = text;
        }

        public void SetPredatorCounter(string text)
        {
            _predatorCounter.text = text;
        }

        public void PlayPreyCounterPulse(float duration, float scale)
        {
            _preyCounter.transform.DOKill();
            _preyCounter.transform.localScale = Vector3.one;
            _preyCounter.transform.DOScale(scale, duration).SetEase(Ease.OutBack);
        }

        public void PlayPredatorCounterPulse(float duration, float scale)
        {
            _predatorCounter.transform.DOKill();
            _predatorCounter.transform.localScale = Vector3.one;
            _predatorCounter.transform.DOScale(scale, duration).SetEase(Ease.OutBack);
        }

        public void ShowTastyLabel(Vector3 screenPosition, float duration, float riseDistance)
        {
            _tastyLabel.ResetLabel();
            _tastyLabel.Play(screenPosition, duration, riseDistance);
        }
    }
}