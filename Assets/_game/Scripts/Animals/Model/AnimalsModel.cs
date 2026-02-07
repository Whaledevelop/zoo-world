using System.Collections.Generic;
using R3;
using ZooWorld.Enums;
using ZooWorld.Events;

namespace ZooWorld.Models
{
    public sealed class AnimalsModel : IAnimalsModel
    {
        private readonly List<IAnimalModel> _animals;
        private readonly ReactiveProperty<int> _alivePreyCount;
        private readonly ReactiveProperty<int> _deadPreyCount;
        private readonly ReactiveProperty<int> _deadPredatorCount;
        private readonly ReadOnlyReactiveProperty<int> _alivePreyCountReadOnly;
        private readonly ReadOnlyReactiveProperty<int> _deadPreyCountReadOnly;
        private readonly ReadOnlyReactiveProperty<int> _deadPredatorCountReadOnly;
        private readonly Subject<IAnimalModel> _animalSpawned;
        private readonly Subject<IAnimalModel> _animalDied;
        private readonly Subject<PredatorAteEvent> _predatorAte;
        private readonly Subject<AnimalCollisionEvent> _animalCollision;

        public AnimalsModel()
        {
            _animals = new List<IAnimalModel>();
            _alivePreyCount = new ReactiveProperty<int>(0);
            _deadPreyCount = new ReactiveProperty<int>(0);
            _deadPredatorCount = new ReactiveProperty<int>(0);
            _alivePreyCountReadOnly = _alivePreyCount.ToReadOnlyReactiveProperty();
            _deadPreyCountReadOnly = _deadPreyCount.ToReadOnlyReactiveProperty();
            _deadPredatorCountReadOnly = _deadPredatorCount.ToReadOnlyReactiveProperty();
            _animalSpawned = new Subject<IAnimalModel>();
            _animalDied = new Subject<IAnimalModel>();
            _predatorAte = new Subject<PredatorAteEvent>();
            _animalCollision = new Subject<AnimalCollisionEvent>();
        }

        public IReadOnlyList<IAnimalModel> Animals => _animals;
        public ReadOnlyReactiveProperty<int> AlivePreyCount => _alivePreyCountReadOnly;
        public ReadOnlyReactiveProperty<int> DeadPreyCount => _deadPreyCountReadOnly;
        public ReadOnlyReactiveProperty<int> DeadPredatorCount => _deadPredatorCountReadOnly;
        public Observable<IAnimalModel> OnAnimalSpawned => _animalSpawned;
        public Observable<IAnimalModel> OnAnimalDied => _animalDied;
        public Observable<PredatorAteEvent> OnPredatorAte => _predatorAte;
        public Observable<AnimalCollisionEvent> OnAnimalCollision => _animalCollision;

        public void AddAnimal(IAnimalModel model)
        {
            _animals.Add(model);

            if (model.Group == AnimalGroup.Prey)
            {
                _alivePreyCount.Value += 1;
            }

            _animalSpawned.OnNext(model);
        }

        public void ReportCollision(IAnimalModel first, IAnimalModel second)
        {
            _animalCollision.OnNext(new AnimalCollisionEvent(first, second));
        }

        public void RegisterPredatorEat(IAnimalModel predator, IAnimalModel prey)
        {
            if (!predator.IsAlive.CurrentValue || !prey.IsAlive.CurrentValue)
            {

                return;
            }

            prey.Kill();

            if (prey.Group == AnimalGroup.Prey)
            {
                _alivePreyCount.Value -= 1;
                _deadPreyCount.Value += 1;
            }
            else
            {
                _deadPredatorCount.Value += 1;
            }

            _animalDied.OnNext(prey);
            _predatorAte.OnNext(new PredatorAteEvent(predator, prey));
        }
    }
}