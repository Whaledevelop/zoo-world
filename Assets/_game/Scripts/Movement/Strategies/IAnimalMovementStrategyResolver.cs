using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public interface IAnimalMovementStrategyResolver
    {
        AnimalMovementStrategyAsset Resolve(IAnimalModel animal);
    }
}