using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class AddOrEditProfileViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly int _id;

    public AddOrEditProfileViewModel(IFileDialogService fileDialogService,
        IProfileRepository profileRepository,
        int id)
    {
        _fileDialogService = fileDialogService;
        _profileRepository = profileRepository;
        _id = id;
        BrowseCommand = ReactiveCommand.CreateFromTask(Browse);
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


    private async Task Browse()
    {
        var result = await _fileDialogService.OpenFilePickerAsync("Select settings file", false, new List<FilePickerFileType>
        {
            new(null)
            {
                Patterns = new List<string>{"*.xml"}
            }
        });
        if (result is not null && result.Length == 1)
        {
            DefaultSettingsPath = result[0];
        }
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

        var profile = new Profile()
        {
            Username = Username,
            DefaultSettingsPath = ShouldSelectDefaultSettings ? DefaultSettingsPath : string.Empty,
            IsPasswordSaved = ShouldSavePassword,
            Password = ShouldSavePassword ? Password : string.Empty,
            Notes = Notes
        };

        if (_id == -1)
        {
            _profileRepository.Insert(profile);
        }
        else
        {
            profile.Id = _id;
            _profileRepository.Update(profile);
        }
        
        dialog.Close(true);
    }
}