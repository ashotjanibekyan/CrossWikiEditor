using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class ProfilesViewModel : ViewModelBase
{
    private readonly IProfileRepository _profileRepository;

    public ProfilesViewModel(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
        LoginCommand = ReactiveCommand.Create(Login);
        AddCommand = ReactiveCommand.Create(Add);
        EditCommand = ReactiveCommand.Create(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
        QuickLoginCommand = ReactiveCommand.Create(QuickLogin);
        Username = "";
        Password = "";
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
    }

    private Profile? _selectedProfile = null;

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set => this.RaiseAndSetIfChanged(ref _selectedProfile, value);
    }

    public ObservableCollection<Profile> Profiles { get; set; }
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