using ZooWorld.Services;
using ZooWorld.Settings;

namespace ZooWorld.Movement.Strategies
{
    public readonly struct AnimalMovementContext
    {
        public AnimalMovementContext(
            IViewportBoundsService viewportBoundsService,
            ScreenBoundsSettings screenBoundsSettings,
            float time)
        {
            ViewportBoundsService = viewportBoundsService;
            ScreenBoundsSettings = screenBoundsSettings;
            Time = time;
        }

        public IViewportBoundsService ViewportBoundsService { get; }
        public ScreenBoundsSettings ScreenBoundsSettings { get; }
        public float Time { get; }
    }
}