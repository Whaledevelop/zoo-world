using UnityEngine;

namespace ZooWorld.Views
{
    public sealed class ZooWorldRootView : MonoBehaviour
    {
        [SerializeField]
        private Transform _root;

        public Transform Root => _root;
    }
}