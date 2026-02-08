using ZooWorld.Models;

namespace ZooWorld.Events
{
    public readonly struct AnimalsCollisionEvent
    {
        public AnimalsCollisionEvent(IAnimalModel first, IAnimalModel second)
        {
            First = first;
            Second = second;
        }

        public IAnimalModel First { get; }
        public IAnimalModel Second { get; }
    }
}