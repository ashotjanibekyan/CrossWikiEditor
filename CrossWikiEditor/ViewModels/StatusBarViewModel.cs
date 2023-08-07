using System;
using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;

    public StatusBarViewModel(IDialogService dialogService, IProfileRepository profileRepository)
    {
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        UsernameClickedCommand = ReactiveCommand.CreateFromTask(UsernameClicked);
    }
    
    public string Username { get; set; } = "User: ";
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }

    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(new ProfilesViewModel(_profileRepository));
    }
}