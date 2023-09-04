using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using Serilog;
using CollectionExtensions = CrossWikiEditor.Core.Utils.CollectionExtensions;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class MakeListViewModel : ViewModelBase
{
    private readonly ILogger _logger;
    private readonly IDialogService _dialogService;
    private readonly IWikiClientCache _clientCache;
    private readonly IPageService _pageService;
    private readonly ISystemService _systemService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IFileDialogService _fileDialogService;
    private readonly IUserPreferencesService _userPreferencesService;

    public MakeListViewModel(
        IMessengerWrapper messenger,
        ILogger logger,
        IDialogService dialogService,
        IWikiClientCache clientCache,
        IPageService pageService,
        ISystemService systemService,
        IViewModelFactory viewModelFactory,
        IFileDialogService fileDialogService,
        IUserPreferencesService userPreferencesService,
        IEnumerable<IListProvider> listProviders)
    {
        _logger = logger;
        _dialogService = dialogService;
        _clientCache = clientCache;
        _pageService = pageService;
        _systemService = systemService;
        _viewModelFactory = viewModelFactory;
        _fileDialogService = fileDialogService;
        _userPreferencesService = userPreferencesService;

        ListProviders = listProviders.ToObservableCollection();
        SelectedListProvider = ListProviders[0];

        messenger.Register<PageUpdatedMessage>(this, (recipient, message) =>
        {
            Pages.Remove(message.Page);
        });
    }

    [RelayCommand]
    private async Task AddNewPage()
    {
        if (!string.IsNullOrWhiteSpace(NewPageTitle))
        {
            Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(_userPreferencesService.GetCurrentPref().UrlApi(), NewPageTitle);
            if (result is {IsSuccessful: true, Value: not null})
            {
                Pages.Add(result.Value);
            }
        }

        NewPageTitle = string.Empty;
    }

    [RelayCommand]
    private void Remove()
    {
        if (Enumerable.Any<WikiPageModel>(SelectedPages))
        {
            CollectionExtensions.Remove(Pages, new List<WikiPageModel>(SelectedPages));
        }

        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    [RelayCommand]
    private async Task MakeList(CancellationToken arg)
    {
        if (SelectedListProvider is INeedAdditionalParamsListProvider needAdditionalParamsListProvider)
        {
            await needAdditionalParamsListProvider.GetAdditionalParams();
        }

        if (!SelectedListProvider.CanMake)
        {
            return;
        }
        
        Result<List<WikiPageModel>> result = SelectedListProvider switch
        {
            ILimitedListProvider limitedListProvider => await limitedListProvider.MakeList(await limitedListProvider.GetLimit()),
            IUnlimitedListProvider unlimitedListProvider => await unlimitedListProvider.MakeList(),
            _ => Result<List<WikiPageModel>>.Success(new List<WikiPageModel>())
        };

        if (result is { IsSuccessful: true, Value: not null })
        {
            CollectionExtensions.AddRange(Pages, result.Value);
        }
        else
        {
            await _dialogService.Alert("Failed to get the list",
                result.Error ?? "Failed to get the list. Please make sure you have internet access.");
        }
    }

    [RelayCommand]
    private void OpenInBrowser()
    {
        if (!Enumerable.Any<WikiPageModel>(SelectedPages))
        {
            return;
        }

        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser($"{_userPreferencesService.GetCurrentPref().UrlIndex()}title={selectedPage.Title}");
        }
    }

    [RelayCommand]
    private void OpenHistoryInBrowser()
    {
        if (!Enumerable.Any<WikiPageModel>(SelectedPages))
        {
            return;
        }

        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser($"{_userPreferencesService.GetCurrentPref().UrlIndex()}title={selectedPage.Title}&action=history");
        }
    }

    [RelayCommand]
    private async Task Cut()
    {
        if (!Enumerable.Any<WikiPageModel>(SelectedPages))
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, Enumerable.Select<WikiPageModel, string>(SelectedPages, x => x.Title)));
        CollectionExtensions.Remove(Pages, Enumerable.ToList<WikiPageModel>(SelectedPages));
        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    [RelayCommand]
    private async Task Copy()
    {
        if (!Enumerable.Any<WikiPageModel>(SelectedPages))
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, Enumerable.Select<WikiPageModel, string>(SelectedPages, x => x.Title)));
    }

    [RelayCommand]
    private async Task Paste()
    {
        string? clipboardText = await _systemService.GetClipboardTextAsync();
        if (!string.IsNullOrWhiteSpace(clipboardText))
        {
            string[] titles = clipboardText.Split(new[] { Environment.NewLine },
                StringSplitOptions.None);
            string urlApi = _userPreferencesService.GetCurrentPref().UrlApi();
            foreach (string title in titles)
            {
                Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(urlApi, title);
                if (result is {IsSuccessful: true, Value: not null})
                {
                    Pages.Add(result.Value);
                }
            }
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (WikiPageModel page in Pages)
        {
            SelectedPages.Add(page);
        }
    }

    [RelayCommand]
    private void SelectNone()
    {
        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    [RelayCommand]
    private void SelectInverse()
    {
        var newSelection = Enumerable.Where<WikiPageModel>(Pages, page => !SelectedPages.Contains(page)).ToList();

        SelectedPages = newSelection.ToObservableCollection();
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        CollectionExtensions.Remove(Pages, Enumerable.ToList<WikiPageModel>(SelectedPages));
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void RemoveAll()
    {
        Pages.Clear();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void RemoveDuplicate()
    {
        Pages = Enumerable.Distinct<WikiPageModel>(Pages).ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void RemoveNonMainSpace()
    {
        Pages = Enumerable.Where<WikiPageModel>(Pages, p => p.NamespaceId == 0).ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void MoveToTop()
    {
        var selectedPages = Enumerable.ToList<WikiPageModel>(SelectedPages);
        CollectionExtensions.Remove(Pages, selectedPages);
        Pages = selectedPages.Concat(Pages).ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void MoveToBottom()
    {
        var selectedPages = Enumerable.ToList<WikiPageModel>(SelectedPages);
        CollectionExtensions.Remove(Pages, selectedPages);
        CollectionExtensions.AddRange(Pages, selectedPages);
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void ConvertToTalkPages()
    {
        List<WikiPageModel>? talkPages = _pageService.ConvertToTalk(Enumerable.ToList<WikiPageModel>(Pages)).Value;
        if (talkPages is not null)
        {
            Pages = talkPages.ToObservableCollection();
        }
    }

    [RelayCommand]
    private void ConvertFromTalkPages()
    {
        List<WikiPageModel>? subjectPages = _pageService.ConvertToSubject(Enumerable.ToList<WikiPageModel>(Pages)).Value;
        if (subjectPages is not null)
        {
            Pages = subjectPages.ToObservableCollection();
        }
    }
    
    [RelayCommand]
    private async Task Filter()
    {
        try
        {
            FilterOptions? result = await _dialogService.ShowDialog<FilterOptions>(await _viewModelFactory.GetFilterViewModel());
            if (result != null)
            {
                var filteredPages = Enumerable.Where<WikiPageModel>(Pages, page => WikiPageExtensions.ShouldKeepPer(page, result)).ToList();

                if (result.SortAlphabetically)
                {
                    filteredPages = filteredPages.OrderBy(x => x.Title).ToList();
                }

                if (result.RemoveDuplicates)
                {
                    filteredPages = filteredPages.Distinct().ToList();
                }

                if (result.Pages.Count != 0)
                {
                    var list = new HashSet<WikiPageModel>(filteredPages);
                    switch (result.SetOperation)
                    {
                        case SetOperations.SymmetricDifference:
                            list.ExceptWith(result.Pages);
                            break;
                        case SetOperations.Intersection:
                            list.IntersectWith(result.Pages);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    filteredPages = new List<WikiPageModel>(list);
                }

                Pages = filteredPages.ToObservableCollection();
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Failed to filter the list");
        }
    }

    [RelayCommand]
    private async Task SaveList()
    {
        string? suggestedTitle =
            $"{SelectedListProvider.Title}_{SelectedListProvider.Param}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.txt";
        var (openReadStream, openWriteStream) = await _fileDialogService.SaveFilePickerAsync("Save pages", suggestedFileName: suggestedTitle.ToFilenameSafe());
        var stream = await openReadStream();
        foreach ((string title, int _) in Pages)
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"# [[:{title}]]{Environment.NewLine}"));
        }

        stream.Close();
    }

    [RelayCommand]
    private void SortAlphabetically()
    {
        Pages = Enumerable.OrderBy<WikiPageModel, string>(Pages, x => x.Title).ToObservableCollection();
    }

    [RelayCommand]
    private void SortReverseAlphabetically()
    {
        Pages = Enumerable.OrderByDescending<WikiPageModel, string>(Pages, x => x.Title).ToObservableCollection();
    }

    [ObservableProperty] private ObservableCollection<IListProvider> _listProviders;
    [ObservableProperty] private IListProvider _selectedListProvider;
    [ObservableProperty] private ObservableCollection<WikiPageModel> _pages = new();
    [ObservableProperty] private ObservableCollection<WikiPageModel> _selectedPages = new();
    [ObservableProperty] private string _newPageTitle = string.Empty;
}