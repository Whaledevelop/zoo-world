using R3;
using UnityEngine;
using ZooWorld.Enums;
using ZooWorld.Settings;

namespace ZooWorld.Models
{
    public sealed class AnimalModel : IAnimalModel
    {
        private AnimalSettings _animalSettings;
        
        private readonly ReactiveProperty<bool> _isAlive;
        private readonly ReactiveProperty<Vector3> _position;
        private readonly ReactiveProperty<Vector3> _velocity;
        private readonly ReadOnlyReactiveProperty<bool> _isAliveReadOnly;
        private readonly ReadOnlyReactiveProperty<Vector3> _positionReadOnly;
        private readonly ReadOnlyReactiveProperty<Vector3> _velocityReadOnly;

        public AnimalModel(int animalId, AnimalSettings animalSettings, Vector3 position)
        {
            Id = animalId;
            _animalSettings = animalSettings;
            _isAlive = new ReactiveProperty<bool>(true);
            _position = new ReactiveProperty<Vector3>(position);
            _velocity = new ReactiveProperty<Vector3>(Vector3.zero);
            _isAliveReadOnly = _isAlive.ToReadOnlyReactiveProperty();
            _positionReadOnly = _position.ToReadOnlyReactiveProperty();
            _velocityReadOnly = _velocity.ToReadOnlyReactiveProperty();
        }

        public int Id { get; }
        public AnimalType Type => _animalSettings.Type;
        public AnimalGroup Group => _animalSettings.Group;

        public AnimalMovementSettings MovementSettings => _animalSettings.AnimalMovementSettings;
        public ReadOnlyReactiveProperty<bool> IsAlive => _isAliveReadOnly;
        public ReadOnlyReactiveProperty<Vector3> Position => _positionReadOnly;
        public ReadOnlyReactiveProperty<Vector3> Velocity => _velocityReadOnly;

        public void SetPosition(Vector3 position)
        {
            _position.Value = position;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _velocity.Value = velocity;
        }

        public void Kill()
        {
            if (!_isAlive.Value)
            {

                return;
            }

            _isAlive.Value = false;
        }
    }
}