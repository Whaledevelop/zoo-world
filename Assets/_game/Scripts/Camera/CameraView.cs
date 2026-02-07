using UnityEngine;

namespace ZooWorld.Views
{
    public sealed class CameraView : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        public Camera Camera => _camera;
    }
}