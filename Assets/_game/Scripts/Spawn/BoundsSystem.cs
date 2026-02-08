﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Settings;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class BoundsSystem : AsyncLifetime, IGameSystem
    {
        private readonly IAnimalsModel _animalsModel;
        private readonly ICameraModel _cameraModel;
        private readonly ScreenBoundsSettings _screenBoundsSettings;

        public BoundsSystem(IAnimalsModel animalsModel, ICameraModel cameraModel, ScreenBoundsSettings screenBoundsSettings)
        {
            _animalsModel = animalsModel;
            _cameraModel = cameraModel;
            _screenBoundsSettings = screenBoundsSettings;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            ClampLoopAsync(cancellationToken).Forget();

            return UniTask.CompletedTask;
        }

        private async UniTask ClampLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ClampAnimals();
                await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancellationToken);
            }
        }

        private void ClampAnimals()
        {
            if (_screenBoundsSettings.UseViewportBounds)
            {

                return;
            }

            var bounds = _cameraModel.WorldBounds.CurrentValue;

            foreach (var animal in _animalsModel.Animals)
            {
                if (!animal.IsAlive.CurrentValue)
                {

                    continue;
                }

                var position = animal.Position.CurrentValue;
                var clampedPosition = new Vector3(
                    Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
                    position.y,
                    Mathf.Clamp(position.z, bounds.min.z, bounds.max.z));

                if (position != clampedPosition)
                {
                    animal.SetPosition(clampedPosition);
                }
            }
        }
    }
}