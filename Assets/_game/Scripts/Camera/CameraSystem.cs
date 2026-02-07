using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Models;
using ZooWorld.Views;
using Whaledevelop;
using Whaledevelop.Systems;

namespace ZooWorld.Systems
{
    public sealed class CameraSystem : AsyncLifetime, IGameSystem
    {
        private readonly ICameraModel _cameraModel;
        private readonly CameraView _cameraView;

        public CameraSystem(ICameraModel cameraModel, CameraView cameraView)
        {
            _cameraModel = cameraModel;
            _cameraView = cameraView;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            ApplySettings();

            return UniTask.CompletedTask;
        }

        private void ApplySettings()
        {
            var camera = _cameraView.Camera;
            var position = _cameraModel.CameraPosition.CurrentValue;
            var rotation = _cameraModel.CameraRotation.CurrentValue;

            camera.transform.position = position;
            camera.transform.rotation = Quaternion.Euler(rotation);
            camera.orthographic = true;
            camera.orthographicSize = _cameraModel.CameraOrthographicSize.CurrentValue;
        }
    }
}