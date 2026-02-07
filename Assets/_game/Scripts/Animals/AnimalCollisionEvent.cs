using ZooWorld.Models;

namespace ZooWorld.Events
{
    public readonly struct AnimalCollisionEvent
    {
        public AnimalCollisionEvent(IAnimalModel first, IAnimalModel second)
        {
            First = first;
            Second = second;
        }

        public IAnimalModel First { get; }
        public IAnimalModel Second { get; }
    }
}