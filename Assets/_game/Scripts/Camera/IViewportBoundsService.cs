using UnityEngine;

namespace ZooWorld.Services
{
    public interface IViewportBoundsService
    {
        bool IsInside(Vector3 worldPosition);

        Vector3 GetReturnDirection(Vector3 worldPosition);
    }
}