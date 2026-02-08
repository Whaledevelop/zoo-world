using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public interface IAnimalMovementStrategyResolver
    {
        IAnimalMovementStrategy Resolve(IAnimalModel animal);
    }
}