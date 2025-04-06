using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Repositories;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class AddOrEditProfileViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly int _id;
    private readonly IProfileRepository _profileRepository;

    public AddOrEditProfileViewModel(
        IFileDialogService fileDialogService,
        IProfileRepository profileRepository,
        int id)
    {
        _fileDialogService = fileDialogService;
        _profileRepository = profileRepository;
        _id = id;
        Username = string.Empty;
        Password = string.Empty;
        DefaultSettingsPath = string.Empty;
        Notes = string.Empty;
    }

    public bool IsEdit => _id != -1;

    [ObservableProperty] public partial string Username { get; set; }
    [ObservableProperty] public partial string Password { get; set; }
    [ObservableProperty] public partial string DefaultSettingsPath { get; set; }
    [ObservableProperty] public partial bool ShouldSavePassword { get; set; }
    [ObservableProperty] public partial bool ShouldSelectDefaultSettings { get; set; }
    [ObservableProperty] public partial string Notes { get; set; }

    [RelayCommand]
    private async Task Browse()
    {
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Select settings file", false, ["*.xml"]);
        if (result?.Length == 1)
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

        var profile = new Profile
        {
            Username = Username,
            DefaultSettingsPath = ShouldSelectDefaultSettings ? DefaultSettingsPath : string.Empty,
            IsPasswordSaved = ShouldSavePassword,
            Password = ShouldSavePassword ? Password : string.Empty,
            Notes = Notes
        };

        if (IsEdit)
        {
            profile.Id = _id;
            _profileRepository.Update(profile);
        }
        else
        {
            _profileRepository.Insert(profile);
        }

        dialog.Close(true);
    }

    [RelayCommand]
    private void Cancel(IDialog dialog)
    {
        dialog.Close(false);
    }
}