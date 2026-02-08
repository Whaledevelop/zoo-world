using ZooWorld.Obstacles;
using ZooWorld.Services;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public readonly struct AnimalMovementContext
    {
        public AnimalMovementContext(
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings,
            IObstacleQueryService obstacleQueryService,
            ObstacleSettings obstacleSettings,
            float time)
        {
            ViewportBoundsService = viewportBoundsService;
            ScreenBoundsSettings = screenBoundsSettings;
            ObstacleQueryService = obstacleQueryService;
            ObstacleSettings = obstacleSettings;
            Time = time;
        }

        public IViewportBoundsService ViewportBoundsService { get; }
        public ScreenBoundsSettings ScreenBoundsSettings { get; }
        public IObstacleQueryService ObstacleQueryService { get; }
        public ObstacleSettings ObstacleSettings { get; }
        public float Time { get; }
    }
}