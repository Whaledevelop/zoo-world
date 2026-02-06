using System.Collections.Generic;
using UnityEngine;

namespace Whaledevelop.UI
{
    public interface IUISettings
    {
        IReadOnlyList<Canvas> CanvasPrefabs { get; }
        
        IReadOnlyList<UIView> UIViewsPrefabs { get; }
        
        IReadOnlyList<RootUIView> RootsPrefabs { get; }
    }
}
