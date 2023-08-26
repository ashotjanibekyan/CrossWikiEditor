using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class MakeListViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IWikiClientCache _clientCache;
    private readonly ISystemService _systemService;
    private readonly IUserPreferencesService _userPreferencesService;

    public MakeListViewModel(IDialogService dialogService,
        IWikiClientCache clientCache,
        ISystemService systemService,
        IUserPreferencesService userPreferencesService,
        IEnumerable<IListProvider> listProviders)
    {
        _dialogService = dialogService;
        _clientCache = clientCache;
        _systemService = systemService;
        _userPreferencesService = userPreferencesService;
        AddNewPageCommand = ReactiveCommand.CreateFromTask(AddNewPage);
        RemoveCommand = ReactiveCommand.Create(Remove);
        MakeListCommand = ReactiveCommand.CreateFromTask(MakeList);
        OpenInBrowserCommand = ReactiveCommand.Create(OpenInBrowser);
        OpenHistoryInBrowserCommand = ReactiveCommand.Create(OpenHistoryInBrowser);
        CutCommand = ReactiveCommand.CreateFromTask(Cut);
        CopyCommand = ReactiveCommand.CreateFromTask(Copy);
        PasteCommand = ReactiveCommand.Create(Paste);
        SelectAllCommand = ReactiveCommand.Create(SelectAll);
        SelectNoneCommand = ReactiveCommand.Create(SelectNone);
        SelectInverseCommand = ReactiveCommand.Create(SelectInverse);
        
        ListProviders = listProviders.ToObservableCollection();
        SelectedListProvider = ListProviders[0];
    }


    private async Task AddNewPage()
    {
        if (!string.IsNullOrWhiteSpace(NewPageTitle))
        {
            Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(_userPreferencesService.GetCurrentPref().UrlApi(), NewPageTitle);
            Pages.Add(result is {IsSuccessful: true, Value: not null} ? result.Value : new WikiPageModel(NewPageTitle.Trim()));
        }

        NewPageTitle = string.Empty;
    }

    private void Remove()
    {
        if (SelectedPages.Any())
        {
            Pages.Remove(new List<WikiPageModel>(SelectedPages));
        }

        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    private async Task MakeList(CancellationToken arg)
    {
        if (SelectedListProvider.NeedsAdditionalParams)
        {
            await SelectedListProvider.GetAdditionalParams();
        }
        
        if (!SelectedListProvider.CanMake)
        {
            return;
        }

        Result<List<WikiPageModel>> result = await SelectedListProvider.MakeList();
        if (result is {IsSuccessful: true, Value: not null})
        {
            Pages.AddRange(result.Value);
        }
        else
        {
            await _dialogService.Alert("Failed to get the list", result.Error ?? "Failed to get the list. Please make sure you have internet access.");
        }
    }

    private void OpenInBrowser()
    {
        if (!SelectedPages.Any())
        {
            return;
        }
        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser($"{_userPreferencesService.GetCurrentPref().UrlIndex()}title={selectedPage.Title}");
        }
    }

    private void OpenHistoryInBrowser()
    {
        if (!SelectedPages.Any())
        {
            return;
        }
        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser($"{_userPreferencesService.GetCurrentPref().UrlIndex()}title={selectedPage.Title}&action=history");
        }
    }

    private async Task Cut()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join('\n', SelectedPages.Select(x => x.Title)));
        Pages.Remove(SelectedPages.ToList());
        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    private async Task Copy()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join('\n', SelectedPages.Select(x => x.Title)));
    }

    private void Paste()
    {
        throw new System.NotImplementedException();
    }

    private void SelectAll()
    {
        throw new System.NotImplementedException();
    }

    private void SelectNone()
    {
        throw new System.NotImplementedException();
    }
    
    private void SelectInverse()
    {
        throw new System.NotImplementedException();
    }

    public ObservableCollection<IListProvider> ListProviders { get; }
    [Reactive] public IListProvider SelectedListProvider { get; set; }
    [Reactive] public ObservableCollection<WikiPageModel> Pages { get; set; } = new();
    [Reactive] public ObservableCollection<WikiPageModel> SelectedPages { get; set; } = new();
    [Reactive] public string NewPageTitle { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> AddNewPageCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
    public ReactiveCommand<Unit, Unit> MakeListCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenInBrowserCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenHistoryInBrowserCommand { get; }
    public ReactiveCommand<Unit, Unit> CutCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectNoneCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectInverseCommand { get; }
}