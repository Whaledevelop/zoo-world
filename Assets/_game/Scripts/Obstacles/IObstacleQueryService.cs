using UnityEngine;

namespace ZooWorld.Obstacles
{
    public interface IObstacleQueryService
    {
        bool IsBlocked(Vector3 position, float radius);
        bool TryFindFreePosition(in Bounds bounds, float radius, int maxAttempts, out Vector3 position);
    }
}