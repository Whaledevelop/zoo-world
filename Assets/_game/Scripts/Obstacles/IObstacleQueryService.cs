using UnityEngine;

namespace ZooWorld.Obstacles
{
    public interface IObstacleQueryService
    {
        bool IsBlocked(Vector3 position, float radius);
        bool IsPathBlocked(Vector3 position, Vector3 direction, float distance, float radius);
        bool TryFindFreePosition(in Bounds bounds, float radius, int maxAttempts, out Vector3 position);
    }
}
