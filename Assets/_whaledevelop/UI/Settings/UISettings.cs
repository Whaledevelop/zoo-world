using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.UI;

namespace Whaledevelop.UI
{
    [CreateAssetMenu(menuName = "Whaledevelop/UI/UISettings", fileName = "UISettings")]
    public class UISettings : ScriptableObject, IUISettings
    {
        [SerializeField]
        private UIView[] _uiViewsPrefabs;

        [SerializeField]
        private RootUIView[] _rootsPrefabs;

        [SerializeField]
        private Canvas[] _canvasPrefabs;

        public IReadOnlyList<UIView> UIViewsPrefabs => _uiViewsPrefabs;
        public IReadOnlyList<RootUIView> RootsPrefabs => _rootsPrefabs;
        public IReadOnlyList<Canvas> CanvasPrefabs => _canvasPrefabs;

#if UNITY_EDITOR
        public void SetUIViewsPrefabs(UIView[] prefabs)
        {
            _uiViewsPrefabs = prefabs;
        }

        public void SetRootViewsPrefabs(RootUIView[] prefabs)
        {
            _rootsPrefabs = prefabs;
        }

        public void SetCanvasPrefabs(Canvas[] prefabs)
        {
            _canvasPrefabs = prefabs;
        }
#endif
    }
}
