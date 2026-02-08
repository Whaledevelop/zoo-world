using System.Collections.Generic;
using R3;
using UnityEngine;
using ZooWorld.Events;

namespace ZooWorld.Models
{
    public interface IAnimalsModel
    {
        IReadOnlyList<IAnimalModel> Animals { get; }
        ReadOnlyReactiveProperty<int> AlivePreyCount { get; }
        ReadOnlyReactiveProperty<int> DeadPreyCount { get; }
        ReadOnlyReactiveProperty<int> DeadPredatorCount { get; }
        Observable<IAnimalModel> OnAnimalSpawned { get; }
        Observable<IAnimalModel> OnAnimalDied { get; }
        Observable<IAnimalModel> OnAnimalRemoved { get; }
        Observable<PredatorAteEvent> OnPredatorAte { get; }
        Observable<AnimalsCollisionEvent> OnAnimalCollision { get; }
        Observable<AnimalObstacleCollisionEvent> OnAnimalObstacleCollision { get; }
        void AddAnimal(IAnimalModel model);
        bool TryGetAnimalById(int id, out IAnimalModel animal);
        void RemoveAnimal(IAnimalModel model);
        void ReportCollision(IAnimalModel first, IAnimalModel second);
        void ReportObstacleCollision(IAnimalModel animal, Vector3 point, Vector3 normal);
        void RegisterPredatorEat(IAnimalModel predator, IAnimalModel prey);
    }
}