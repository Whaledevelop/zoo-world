using UnityEngine;
using ZooWorld.Settings;

namespace ZooWorld.Views
{
    public interface IAnimalViewsPool
    {
        AnimalView Get(AnimalSettings settings, Transform parent, Vector3 position, Quaternion rotation);
        void Return(AnimalSettings settings, AnimalView view);
        void ReturnAll();
    }
}