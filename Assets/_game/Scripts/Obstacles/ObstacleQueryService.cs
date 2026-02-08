using UnityEngine;
using ZooWorld.Settings;

namespace ZooWorld.Obstacles
{
    public sealed class ObstacleQueryService : IObstacleQueryService
    {
        private readonly ObstacleSettings _obstacleSettings;

        public ObstacleQueryService(ObstacleSettings obstacleSettings)
        {
            _obstacleSettings = obstacleSettings;
        }

        public bool IsBlocked(Vector3 position, float radius)
        {
            var mask = _obstacleSettings.ObstaclesMask;

            return Physics.CheckSphere(position, radius, mask, QueryTriggerInteraction.Ignore);
        }

        public bool TryFindFreePosition(in Bounds bounds, float radius, int maxAttempts, out Vector3 position)
        {
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var xPosition = Random.Range(bounds.min.x, bounds.max.x);
                var zPosition = Random.Range(bounds.min.z, bounds.max.z);
                var candidate = new Vector3(xPosition, bounds.center.y, zPosition);

                if (IsBlocked(candidate, radius))
                {
                    continue;
                }

                position = candidate;

                return true;
            }

            position = bounds.center;

            return false;
        }
    }
}