using UnityEngine;

namespace ZooWorld.Views
{
    public interface IAnimalViewsRegistry
    {
        void Register(int animalId, AnimalView view);
        bool TryGet(int animalId, out AnimalView view);
        bool TryGetRigidbody(int animalId, out Rigidbody rigidbody);
        void Unregister(int animalId);
    }
}