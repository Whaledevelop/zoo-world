using R3;
using UnityEngine;
using ZooWorld.Enums;
using ZooWorld.Settings;

namespace ZooWorld.Models
{
    public interface IAnimalModel
    {
        int Id { get; }
        AnimalType Type { get; }
        AnimalGroup Group { get; }
        AnimalMovementSettings MovementSettings { get; }
        ReadOnlyReactiveProperty<bool> IsAlive { get; }
        ReadOnlyReactiveProperty<Vector3> Position { get; }
        ReadOnlyReactiveProperty<Vector3> Velocity { get; }
        void SetPosition(Vector3 position);
        void SetVelocity(Vector3 velocity);
        void Kill();
    }
}