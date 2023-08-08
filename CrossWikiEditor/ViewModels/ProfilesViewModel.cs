using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.Models;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public class ProfilesViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly ICredentialService _credentialService;

    public ProfilesViewModel(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        ICredentialService credentialService)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _credentialService = credentialService;
        LoginCommand = ReactiveCommand.Create(Login);
        AddCommand = ReactiveCommand.CreateFromTask(Add);
        EditCommand = ReactiveCommand.Create(Edit);
        DeleteCommand = ReactiveCommand.Create(Delete);
        QuickLoginCommand = ReactiveCommand.Create(QuickLogin);
        Username = "";
        Password = "";
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
    }


    [Reactive]
    public Profile? SelectedProfile { get; set; }

    [Reactive]
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

    private async Task Add()
    {
        if (await _dialogService.ShowDialog<bool>(new AddNewProfileViewModel(_fileDialogService, _profileRepository, _credentialService)))
        {
            Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
        }
    }

    private void Edit()
    {
        throw new System.NotImplementedException();
    }

    private void Delete()
    {
        if (SelectedProfile is null)
        {
            return;
        }
        _profileRepository.Delete(SelectedProfile.Id);
        SelectedProfile = null;
        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());
    }

    private void QuickLogin()
    {
        throw new System.NotImplementedException();
    }
}