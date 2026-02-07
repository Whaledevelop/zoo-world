using System.Collections.Generic;

namespace ZooWorld.Views
{
    public sealed class AnimalViewsRegistry : IAnimalViewsRegistry
    {
        private readonly Dictionary<int, AnimalView> _viewsById = new();

        public void Register(int animalId, AnimalView view)
        {
            _viewsById[animalId] = view;
        }

        public bool TryGet(int animalId, out AnimalView view)
        {
            var isFound = _viewsById.TryGetValue(animalId, out view);

            return isFound;
        }

        public void Unregister(int animalId)
        {
            _viewsById.Remove(animalId);
        }
    }
}