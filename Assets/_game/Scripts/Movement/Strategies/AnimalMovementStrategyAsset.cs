using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public abstract class AnimalMovementStrategyAsset : ScriptableObject
    {
        public abstract void Tick(IAnimalModel animal, Rigidbody rigidbody, in AnimalMovementContext context);

        public virtual void Cleanup(int animalId)
        {
        }
    }
}