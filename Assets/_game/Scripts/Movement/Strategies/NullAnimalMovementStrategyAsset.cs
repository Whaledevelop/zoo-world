using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Movement.Strategies
{
    public sealed class NullAnimalMovementStrategyAsset : AnimalMovementStrategyAsset
    {
        public override void Tick(IAnimalModel animal, Rigidbody rigidbody, in AnimalMovementContext context)
        {
        }
    }
}