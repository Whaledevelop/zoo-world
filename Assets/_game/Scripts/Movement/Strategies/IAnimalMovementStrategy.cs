using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public interface IAnimalMovementStrategy
    {
        void Tick(IAnimalModel animal, Rigidbody rigidbody, float time);
        void Cleanup(int animalId);
    }
}