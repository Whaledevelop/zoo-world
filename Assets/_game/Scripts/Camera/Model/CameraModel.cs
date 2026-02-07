using R3;
using UnityEngine;

namespace ZooWorld.Models
{
    public sealed class CameraModel : ICameraModel
    {
        private readonly ReactiveProperty<Bounds> _worldBounds;
        private readonly ReactiveProperty<Vector3> _cameraPosition;
        private readonly ReactiveProperty<Vector3> _cameraRotation;
        private readonly ReactiveProperty<float> _cameraOrthographicSize;
        private readonly ReadOnlyReactiveProperty<Bounds> _worldBoundsReadOnly;
        private readonly ReadOnlyReactiveProperty<Vector3> _cameraPositionReadOnly;
        private readonly ReadOnlyReactiveProperty<Vector3> _cameraRotationReadOnly;
        private readonly ReadOnlyReactiveProperty<float> _cameraOrthographicSizeReadOnly;

        public CameraModel(Bounds worldBounds, Vector3 cameraPosition, Vector3 cameraRotation, float cameraOrthographicSize)
        {
            _worldBounds = new ReactiveProperty<Bounds>(worldBounds);
            _cameraPosition = new ReactiveProperty<Vector3>(cameraPosition);
            _cameraRotation = new ReactiveProperty<Vector3>(cameraRotation);
            _cameraOrthographicSize = new ReactiveProperty<float>(cameraOrthographicSize);
            _worldBoundsReadOnly = _worldBounds.ToReadOnlyReactiveProperty();
            _cameraPositionReadOnly = _cameraPosition.ToReadOnlyReactiveProperty();
            _cameraRotationReadOnly = _cameraRotation.ToReadOnlyReactiveProperty();
            _cameraOrthographicSizeReadOnly = _cameraOrthographicSize.ToReadOnlyReactiveProperty();
        }

        public ReadOnlyReactiveProperty<Bounds> WorldBounds => _worldBoundsReadOnly;
        public ReadOnlyReactiveProperty<Vector3> CameraPosition => _cameraPositionReadOnly;
        public ReadOnlyReactiveProperty<Vector3> CameraRotation => _cameraRotationReadOnly;
        public ReadOnlyReactiveProperty<float> CameraOrthographicSize => _cameraOrthographicSizeReadOnly;
    }
}