using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Events
{
    public readonly struct AnimalObstacleCollisionEvent
    {
        public AnimalObstacleCollisionEvent(IAnimalModel animal, Vector3 point, Vector3 normal)
        {
            Animal = animal;
            Point = point;
            Normal = normal;
        }

        public IAnimalModel Animal { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
    }
}