using UnityEngine;

namespace Whaledevelop.UI
{
    public abstract class UIView : MonoBehaviour, ILifetime
    {
        public ViewId ViewId => new(gameObject.name);
    }
    
    public abstract class UIView<T> : UIView where T : IUIViewModel
    {
        public T Model { get; set; }
    }
}