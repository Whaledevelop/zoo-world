using System.Collections.Generic;
using R3;
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
        Observable<PredatorAteEvent> OnPredatorAte { get; }
        Observable<AnimalCollisionEvent> OnAnimalCollision { get; }
        void AddAnimal(IAnimalModel model);
        void ReportCollision(IAnimalModel first, IAnimalModel second);
        void RegisterPredatorEat(IAnimalModel predator, IAnimalModel prey);
    }
}