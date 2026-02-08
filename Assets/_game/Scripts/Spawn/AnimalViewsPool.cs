using System.Collections.Generic;
using UnityEngine;
using Whaledevelop;
using ZooWorld.Settings;

namespace ZooWorld.Views
{
    public sealed class AnimalViewsPool : IAnimalViewsPool
    {
        private readonly PoolingSettings _poolingSettings;
        private readonly Dictionary<AnimalSettings, ComponentsPool<AnimalView>> _poolsBySettings;

        public AnimalViewsPool(PoolingSettings poolingSettings)
        {
            _poolingSettings = poolingSettings;
            _poolsBySettings = new Dictionary<AnimalSettings, ComponentsPool<AnimalView>>();
        }

        public AnimalView Get(AnimalSettings settings, Transform parent, Vector3 position, Quaternion rotation)
        {
            var pool = GetOrCreatePool(settings, parent);
            var view = pool.Get();
            view.transform.SetPositionAndRotation(position, rotation);

            return view;
        }

        public void Return(AnimalSettings settings, AnimalView view)
        {
            var pool = GetOrCreatePool(settings, view.transform.parent);
            view.ResetForPool();
            pool.Return(view);
        }

        public void ReturnAll()
        {
            foreach (var pool in _poolsBySettings.Values)
            {
                pool.ReturnAll();
            }
        }

        private ComponentsPool<AnimalView> GetOrCreatePool(AnimalSettings settings, Transform parent)
        {
            if (_poolsBySettings.TryGetValue(settings, out var pool))
            {

                return pool;
            }

            pool = new ComponentsPool<AnimalView>(
                settings.Prefab,
                parent,
                _poolingSettings.PrewarmPerAnimal,
                _poolingSettings.MaxSizePerAnimal);
            _poolsBySettings.Add(settings, pool);

            return pool;
        }
    }
}