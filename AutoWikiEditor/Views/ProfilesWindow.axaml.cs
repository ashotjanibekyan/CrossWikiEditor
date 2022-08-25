using System.ComponentModel;
using AutoWikiEditor.ViewModels;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace AutoWikiEditor.Views
{
    public partial class ProfilesWindow : ReactiveWindow<ProfilesWindowViewModel>
    {
        private bool _test = false;
        public ProfilesWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_test)
            {
                base.OnClosing(e);
            }
            else
            {
                _test = true;
                
                Close(ViewModel!.Result);
            }
        }
    }
}
