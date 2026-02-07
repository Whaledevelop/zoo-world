namespace Whaledevelop.UI
{
    public class UGUIViewRuntimeData
    {
        public UGUIViewRuntimeData(UIView prefab, UIView instance, IUIViewModel viewModel)
        {
            Prefab = prefab;
            Instance = instance;
            ViewModel = viewModel;
        }

        public ViewId ViewId => Prefab.ViewId;
        public UIView Prefab { get; }
        public UIView Instance { get; }
        
        public IUIViewModel ViewModel { get; }
    }
}