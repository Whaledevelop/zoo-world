using VContainer;

namespace Whaledevelop.UI
{
    public static class UIViewUtility
    {
        public static void InitializeAndInjectModel<T>(IObjectResolver objectResolver, UIView uiView, T viewModel) where T : IUIViewModel
        {
            objectResolver.Inject(viewModel);
            uiView.Model = viewModel;
            uiView.Initialize();
        }
        
        public static void InitializeAndInjectModel<T>(this UIView uiView, IObjectResolver objectResolver, T viewModel) where T : IUIViewModel
        {
            objectResolver.Inject(viewModel);
            uiView.Model = viewModel;
            uiView.Initialize();
        }
        
        public static void Initialize(this UIView uiView, IUIViewModel viewModel)
        {
            uiView.Model = viewModel;
            uiView.Initialize();
        }
    }
}