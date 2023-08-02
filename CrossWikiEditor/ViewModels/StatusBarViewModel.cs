using System;
using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public StatusBarViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        UsernameClickedCommand = ReactiveCommand.CreateFromTask(UsernameClicked);
    }
    
    public string Username { get; set; } = "User: ";
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }

    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(new ProfilesViewModel());
    }
}