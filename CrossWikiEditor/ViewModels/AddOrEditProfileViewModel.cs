using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed partial class AddOrEditProfileViewModel(IFileDialogService fileDialogService,
        IProfileRepository profileRepository,
        int id)
    : ViewModelBase
{
    public bool IsEdit => id != -1;

    [Reactive] public string Username { get; set; } = string.Empty;

    [Reactive] public string Password { get; set; } = string.Empty;

    [Reactive] public string DefaultSettingsPath { get; set; } = string.Empty;

    [Reactive] public bool ShouldSavePassword { get; set; }

    [Reactive] public bool ShouldSelectDefaultSettings { get; set; }

    [Reactive] public string Notes { get; set; } = string.Empty;

    [RelayCommand]
    private async Task Browse()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Select settings file", false, new List<FilePickerFileType>
        {
            new(null)
            {
                Patterns = new List<string> {"*.xml"}
            }
        });
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
    private void Cancel(IDialog dialog) => dialog.Close(false);
}