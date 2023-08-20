using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public sealed class StatusBarViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserService _userService;

    public StatusBarViewModel(IFileDialogService fileDialogService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IUserService userService)
    {
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _profileRepository = profileRepository;
        _userService = userService;
        UsernameClickedCommand = ReactiveCommand.CreateFromTask(UsernameClicked);
        CurrentWikiClickedCommand = ReactiveCommand.CreateFromTask(CurrentWikiClicked);
    }
    
    public string Username { get; set; } = "User: ";
    public string CurrentWiki { get; set; } = "hy.wikipedia.org";
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }
    public ReactiveCommand<Unit, Unit> CurrentWikiClickedCommand { get; }

    private async Task UsernameClicked()
    {
        await _dialogService.ShowDialog<bool>(new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService));
    }
    
    private async Task CurrentWikiClicked()
    {
        await _dialogService.ShowDialog<bool>(new PreferencesViewModel());
    }
}