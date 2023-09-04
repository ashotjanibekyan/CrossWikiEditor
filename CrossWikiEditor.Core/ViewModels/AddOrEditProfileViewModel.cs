using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class AddOrEditProfileViewModel(IFileDialogService fileDialogService,
        IProfileRepository profileRepository,
        int id)
    : ViewModelBase
{
    public bool IsEdit => id != -1;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _defaultSettingsPath = string.Empty;
    [ObservableProperty] private bool _shouldSavePassword;
    [ObservableProperty] private bool _shouldSelectDefaultSettings;
    [ObservableProperty] private string _notes = string.Empty;

    [RelayCommand]
    private async Task Browse()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select settings file", false, new List<string> {"*.xml"});
        if (result is not null && result.Length == 1)
        {
            DefaultSettingsPath = result[0];
        }
    }

    [RelayCommand]
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

        if (IsEdit)
        {
            profile.Id = id;
            profileRepository.Update(profile);
        }
        else
        {
            profileRepository.Insert(profile);
        }

        dialog.Close(true);
    }

    [RelayCommand]
    private void Cancel(IDialog dialog)
    {
        dialog.Close(false);
    }
}