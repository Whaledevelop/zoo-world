using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Whaledevelop.UI
{
    public class RootUIView : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(nameof(ValidateEntries))]
        private SerializableDictionary<UIView, RectTransform> _viewsContainers = new();

        public bool TryGetUIViewContainer(UIView viewPrefab, out RectTransform point)
        {
            var hasPoint = _viewsContainers.TryGetValue(viewPrefab, out point);

            return hasPoint;
        }
        
        private bool ValidateEntries(SerializableDictionary<UIView, RectTransform> views, ref string message)
        {
            var keys = views.Keys.ToArray();
            foreach (var keyValuePair in views)
            {
                if (keyValuePair.Key == null)
                {
                    message = $"One of keys is null";
                    return false;
                }
                if (keyValuePair.Value == null)
                {
                    message = $"Value with key {keyValuePair.Key} is null";
                    return false;
                }
            }
            return true;
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium)]
        private void CreateAllViews()
        {
            foreach (var (viewPrefab, container) in _viewsContainers)
            {
                PrefabUtility.InstantiatePrefab(viewPrefab.gameObject, container);
            }
        }
#endif
    }
}
