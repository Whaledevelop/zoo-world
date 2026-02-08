using ZooWorld.Obstacles;
using ZooWorld.Services;
using ZooWorld.Settings;
using ZooWorld.Views;

namespace ZooWorld.Movement.Strategies
{
    public readonly struct AnimalMovementContext
    {
        public AnimalMovementContext(
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings,
            IObstacleQueryService obstacleQueryService,
            ObstacleSettings obstacleSettings,
            IAnimalViewsRegistry viewsRegistry,
            float time,
            float deltaTime)
        {
            ViewportBoundsService = viewportBoundsService;
            ScreenBoundsSettings = screenBoundsSettings;
            ObstacleQueryService = obstacleQueryService;
            ObstacleSettings = obstacleSettings;
            ViewsRegistry = viewsRegistry;
            Time = time;
            DeltaTime = deltaTime;
        }

        public IViewportBoundsService ViewportBoundsService { get; }
        public ScreenBoundsSettings ScreenBoundsSettings { get; }
        public IObstacleQueryService ObstacleQueryService { get; }
        public ObstacleSettings ObstacleSettings { get; }
        public IAnimalViewsRegistry ViewsRegistry { get; }
        public float Time { get; }
        public float DeltaTime { get; }
    }
}