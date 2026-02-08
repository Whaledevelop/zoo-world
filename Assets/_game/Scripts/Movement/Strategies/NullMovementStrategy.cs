using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public sealed class NullAnimalMovementStrategy : IAnimalMovementStrategy
    {
        public void Tick(IAnimalModel animal, Rigidbody rigidbody, float time)
        {
        }

        public void Cleanup(int animalId)
        {
        }
    }
}