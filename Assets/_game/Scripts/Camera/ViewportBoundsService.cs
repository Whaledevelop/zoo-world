using UnityEngine;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Services
{
    public sealed class ViewportBoundsService : IViewportBoundsService
    {
        private readonly CameraView _cameraView;
        private readonly ScreenBoundsSettings _settings;

        public ViewportBoundsService(CameraView cameraView, ScreenBoundsSettings settings)
        {
            _cameraView = cameraView;
            _settings = settings;
        }

        public bool IsInside(Vector3 worldPosition)
        {
            var viewportPosition = _cameraView.Camera.WorldToViewportPoint(worldPosition);
            var padding = _settings.ViewportPadding;
            var isInside = viewportPosition.z > 0f
                           && viewportPosition.x >= padding
                           && viewportPosition.x <= 1f - padding
                           && viewportPosition.y >= padding
                           && viewportPosition.y <= 1f - padding;

            return isInside;
        }

        public Vector3 GetReturnDirection(Vector3 worldPosition)
        {
            var camera = _cameraView.Camera;
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);

            if (viewportPosition.z <= 0f)
            {
                var cameraForward = camera.transform.forward;
                var flattenedForward = new Vector3(cameraForward.x, 0f, cameraForward.z);
                var direction = flattenedForward.normalized;

                return direction;
            }

            var depth = viewportPosition.z;
            var centerWorld = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, depth));
            var directionToCenter = centerWorld - worldPosition;
            directionToCenter.y = 0f;

            return directionToCenter.normalized;
        }
    }
}