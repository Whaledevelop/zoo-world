using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whaledevelop.UI
{
    [Serializable]
    public class UIToolkitViewData : IViewData
    {
        [field: SerializeField]
        public VisualTreeAsset UxmlAsset { get; private set; }
        
        [field: SerializeField]
        public StyleSheet[] StyleSheets { get; private set; }
    }
}