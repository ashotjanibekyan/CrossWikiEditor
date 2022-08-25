using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using AutoWikiEditor.Services.Interfaces;
using Splat;

namespace AutoWikiEditor.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    public StatusBarViewModel()
    {
        this._dialogService = Locator.Current.GetService<IDialogService>();
        this.UserName = "User";
        this.UserNameClickedCommand = ReactiveCommand.CreateFromTask(this.UserNameClicked);
    }

    public ReactiveCommand<Unit, Unit> UserNameClickedCommand { get; }
    public string UserName { get; }

    private async Task UserNameClicked()
    {
        var resilt = await this._dialogService.ShowDialog<object, ProfilesWindowViewModel>();

        int f = 32;
    }
}
