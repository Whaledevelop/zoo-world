using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public sealed class AnimalMovementStrategyResolver : IAnimalMovementStrategyResolver
    {
        private readonly CatalogAnimalMovementStrategyResolver _resolver;

        public AnimalMovementStrategyResolver(
            AnimalMovementStrategyCatalog catalog,
            NullAnimalMovementStrategyAsset fallback)
        {
            _resolver = new CatalogAnimalMovementStrategyResolver(catalog, fallback);
        }

        public AnimalMovementStrategyAsset Resolve(IAnimalModel animal)
        {
            var strategy = _resolver.Resolve(animal);

            return strategy;
        }
    }
}