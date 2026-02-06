using UnityEngine;

namespace Whaledevelop.UI
{
    public class UIView : MonoBehaviour
    {
        public IUIViewModel Model;

        public virtual void Initialize()
        {
        }

        public virtual void Release()
        {
        }
    }
    
    public abstract class UIView<T> : UIView where T : IUIViewModel
    {
        protected T DerivedModel => (T)Model;
    }
}