using ZooWorld.Models;

namespace ZooWorld.Events
{
    public readonly struct PredatorAteEvent
    {
        public PredatorAteEvent(IAnimalModel predator, IAnimalModel prey)
        {
            Predator = predator;
            Prey = prey;
        }

        public IAnimalModel Predator { get; }
        public IAnimalModel Prey { get; }
    }
}