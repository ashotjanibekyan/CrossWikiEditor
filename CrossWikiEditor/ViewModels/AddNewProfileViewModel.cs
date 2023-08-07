using System.Reactive;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public class AddNewProfileViewModel : ViewModelBase
{
    private readonly IProfileRepository _profileRepository;
    private readonly ICredentialService _credentialService;

    public AddNewProfileViewModel(IProfileRepository profileRepository, ICredentialService credentialService)
    {
        _profileRepository = profileRepository;
        _credentialService = credentialService;
        BrowseCommand = ReactiveCommand.Create(Browse);
        SaveCommand = ReactiveCommand.Create<IDialog>(Save);
        CancelCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(false));
    }

    [Reactive] 
    public string Username { get; set; } = string.Empty;

    [Reactive] 
    public string Password { get; set; } = string.Empty;

    [Reactive] 
    public string DefaultSettingsPath { get; set; } = string.Empty;
    
    [Reactive]
    public bool ShouldSavePassword { get; set; }
    
    [Reactive]
    public bool ShouldSelectDefaultSettings { get; set; }
    
    [Reactive]
    public string Notes { get; set; } = string.Empty;
    
    public ReactiveCommand<Unit, Unit> BrowseCommand { get; }
    public ReactiveCommand<IDialog, Unit> SaveCommand { get; }
    public ReactiveCommand<IDialog, Unit> CancelCommand { get; }


    private void Browse()
    {
        throw new System.NotImplementedException();
    }
    
    private void Save(IDialog dialog)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            dialog.Close(false);
            return;
        }

        if (ShouldSavePassword && string.IsNullOrWhiteSpace(Password))
        {
            dialog.Close(false);
            return;
        }

        if (ShouldSelectDefaultSettings && string.IsNullOrWhiteSpace(DefaultSettingsPath))
        {
            dialog.Close(false);
            return;
        }

        _profileRepository.Insert(new Profile
        {
            Username = Username,
            DefaultSettingsPath = DefaultSettingsPath,
            IsPasswordSaved = ShouldSavePassword,
            Notes = Notes
        });

        if (ShouldSavePassword)
        {
            _credentialService.SavePassword(Username, Password);
        }
        dialog.Close(true);
    }
}