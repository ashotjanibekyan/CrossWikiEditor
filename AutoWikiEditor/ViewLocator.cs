using System;
using Avalonia.Controls;
using AutoWikiEditor.ViewModels;
using Splat;

namespace AutoWikiEditor
{
    public class ViewLocator
    {
        private readonly IReadonlyDependencyResolver _resolver;

        public ViewLocator(IReadonlyDependencyResolver resolver)
        {
            this._resolver = resolver;
        }
        
        public Window GetWindowFromViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            return GetViewFromViewModel<Window, TViewModel>();
        }

        private TView GetViewFromViewModel<TView, TViewModel>() where TViewModel : ViewModelBase
                                                                where TView : IContentControl
        {
            var viewModel = this._resolver.GetService<TViewModel>();

            var vmName = viewModel.GetType().FullName!.Replace("ViewModel", "View").Replace("WindowView", "Window");
            var viewType = Type.GetType(vmName);

            if (viewType == null)
            {
                throw new Exception("Can't resolve a view");
            }

            var view = (TView)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;
            return view;
        }
    }
}