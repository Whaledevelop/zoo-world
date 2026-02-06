using System;
using QuizRPG;
using UnityEngine;
using Whaledevelop.Services;

namespace Whaledevelop.UI
{
    public interface IUIService : IService
    {
        bool TryGetRoot(int rootIndex, out RootUIView root);

        bool TryGetModel<TModel>(int viewIndex, out TModel model) where TModel : IUIViewModel;

        bool TryGetModelOfType<TModel>(out TModel model);

        bool TryGetView<TView>(int viewIndex, out TView view) where TView : UIView;

        bool TryOpenView(int viewIndex, IUIViewModel viewModel, int rootIndex = -1, int canvasIndex = 0);

        bool TryCloseView(int viewIndex);

        bool TryOpenRoot(int rootIndex, int canvasIndex = 0);

        bool TryCloseRoot(int rootIndex);

        event Action<int> OnViewOpened;
        
        event Action<int> OnViewClosed;
    }
}