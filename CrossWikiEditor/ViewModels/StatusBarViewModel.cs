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
    private readonly ICredentialService _credentialService;

    public StatusBarViewModel(IDialogService dialogService, IProfileRepository profileRepository, ICredentialService credentialService)
    {
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _credentialService = credentialService;
        UsernameClickedCommand = ReactiveCommand.CreateFromTask(UsernameClicked);
    }
    
    public string Username { get; set; } = "User: ";
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }

    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(new ProfilesViewModel(_dialogService, _profileRepository, _credentialService));
    }
}