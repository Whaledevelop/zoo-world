using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Whaledevelop.UI
{
    public class DefaultUIToolkitViewOpener : IUIToolkitViewOpener
    {
        private readonly VisualElement _root;

        public DefaultUIToolkitViewOpener(VisualElement root)
        {
            _root = root;
        }

        public UniTask OpenAsync(UIToolkitViewData viewData, CancellationToken cancellationToken = default)
        {
            var element = viewData.UxmlAsset.CloneTree();

            foreach (var styleSheet in viewData.StyleSheets)
            {
                element.styleSheets.Add(styleSheet);
            }

            _root.Add(element);

            
            return UniTask.CompletedTask;
        }
    }
}
