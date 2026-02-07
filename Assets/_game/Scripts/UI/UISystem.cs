using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using ZooWorld.Events;
using ZooWorld.Models;
using ZooWorld.Settings;
using ZooWorld.Views;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class UISystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly ZooUIView _view;
        private readonly UISettings _uiSettings;
        private readonly PredatorEatSettings _predatorEatSettings;
        private readonly List<IDisposable> _subscriptions;

        public UISystem(IAnimalsModel animalsModel, ZooUIView view, UISettings uiSettings, PredatorEatSettings predatorEatSettings)
        {
            _animalsModel = animalsModel;
            _view = view;
            _uiSettings = uiSettings;
            _predatorEatSettings = predatorEatSettings;
            _subscriptions = new List<IDisposable>();
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _subscriptions.Add(_animalsModel.DeadPreyCount.Subscribe(UpdatePreyCounter));
            _subscriptions.Add(_animalsModel.DeadPredatorCount.Subscribe(UpdatePredatorCounter));
            _subscriptions.Add(_animalsModel.OnPredatorAte.Subscribe(OnPredatorAte));

            return UniTask.CompletedTask;
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();

            return UniTask.CompletedTask;
        }

        private void UpdatePreyCounter(int count)
        {
            var text = string.Format(_uiSettings.PreyCounterFormat, count);
            _view.SetPreyCounter(text);
            _view.PlayPreyCounterPulse(_uiSettings.CounterTweenDuration, _uiSettings.CounterPunchScale);
        }

        private void UpdatePredatorCounter(int count)
        {
            var text = string.Format(_uiSettings.PredatorCounterFormat, count);
            _view.SetPredatorCounter(text);
            _view.PlayPredatorCounterPulse(_uiSettings.CounterTweenDuration, _uiSettings.CounterPunchScale);
        }

        private void OnPredatorAte(PredatorAteEvent eatEvent)
        {
            var worldPosition = eatEvent.Predator.Position.CurrentValue + _predatorEatSettings.TastyLabelWorldOffset;
            var screenPosition = _view.UICamera.WorldToScreenPoint(worldPosition);
            _view.ShowTastyLabel(screenPosition, _predatorEatSettings.TastyLabelDuration, _predatorEatSettings.TastyLabelRiseDistance);
        }
    }
}