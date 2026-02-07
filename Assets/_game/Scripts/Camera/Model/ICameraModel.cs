using R3;
using UnityEngine;

namespace ZooWorld.Models
{
    public interface ICameraModel
    {
        ReadOnlyReactiveProperty<Bounds> WorldBounds { get; }
        ReadOnlyReactiveProperty<Vector3> CameraPosition { get; }
        ReadOnlyReactiveProperty<Vector3> CameraRotation { get; }
        ReadOnlyReactiveProperty<float> CameraOrthographicSize { get; }
    }
}