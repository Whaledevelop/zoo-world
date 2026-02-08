using UnityEngine;

namespace ZooWorld.Views
{
    public sealed class ZooWorldRootView : MonoBehaviour
    {
        [SerializeField]
        private Transform _animalsSpawnRoot;

        public Transform AnimalsSpawnRoot => _animalsSpawnRoot;
    }
}