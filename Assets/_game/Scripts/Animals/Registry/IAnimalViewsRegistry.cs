namespace ZooWorld.Views
{
    public interface IAnimalViewsRegistry
    {
        void Register(int animalId, AnimalView view);
        bool TryGet(int animalId, out AnimalView view);
        void Unregister(int animalId);
    }
}