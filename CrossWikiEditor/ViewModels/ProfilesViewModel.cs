using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CrossWikiEditor.Models;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class ProfilesViewModel : ViewModelBase
{
    public ProfilesViewModel()
    {
        LoginCommand = ReactiveCommand.Create(Login);
        AddCommand = ReactiveCommand.Create(Add);
        EditCommand = ReactiveCommand.Create(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
        QuickLoginCommand = ReactiveCommand.Create(QuickLogin);
        Username = "";
        Password = "";
    }

    private Profile? _selectedProfile = null;

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set => this.RaiseAndSetIfChanged(ref _selectedProfile, value);
    }

    public ObservableCollection<Profile> Profiles { get; set; } =  new(new List<Profile>
    {
        new Profile()
        {
            Id = 23,
            Username = "ashot",
            Notes = "fwefw",
            IsPasswordSaved = true,
            DefaultSettingsPath = "fewfwe"
        },
        new Profile()
        {
            Id = 23,
            Username = "ashot",
            Notes = "fwefw",
            IsPasswordSaved = false,
            DefaultSettingsPath = "fewfwe"
        },
    });
    
    public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
    public ReactiveCommand<Unit, Unit> QuickLoginCommand { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }

    private void Login()
    {
        throw new System.NotImplementedException();
    }

    private void Add()
    {
        throw new System.NotImplementedException();
    }

    private void Edit()
    {
        throw new System.NotImplementedException();
    }

    private void Delete()
    {
        throw new System.NotImplementedException();
    }

    private void QuickLogin()
    {
        throw new System.NotImplementedException();
    }
}