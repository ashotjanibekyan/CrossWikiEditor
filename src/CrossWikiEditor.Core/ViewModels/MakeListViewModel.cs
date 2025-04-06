using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.Utils.Extensions;
using Serilog;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class MakeListViewModel : ViewModelBase
{
    private readonly IWikiClientCache _clientCache;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly ILogger _logger;
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly ISystemService _systemService;
    private readonly IViewModelFactory _viewModelFactory;

    public MakeListViewModel(
        IMessengerWrapper messenger,
        ILogger logger,
        IDialogService dialogService,
        IWikiClientCache clientCache,
        IPageService pageService,
        ISystemService systemService,
        IViewModelFactory viewModelFactory,
        IFileDialogService fileDialogService,
        ISettingsService settingsService,
        IEnumerable<IListProvider> listProviders)
    {
        _logger = logger;
        _dialogService = dialogService;
        _clientCache = clientCache;
        _pageService = pageService;
        _systemService = systemService;
        _viewModelFactory = viewModelFactory;
        _fileDialogService = fileDialogService;
        _settingsService = settingsService;

        ListProviders = listProviders.OrderBy(l => l.Title).ToObservableCollection();
        SelectedListProvider = ListProviders[0];

        messenger.Register<PageUpdatedMessage>(this, (recipient, message) => Pages.Remove(Pages.First(p => p.Title == message.Page.Title)));
        messenger.Register<PageSkippedMessage>(this, (recipient, message) => Pages.Remove(Pages.First(p => p.Title == message.Page.Title)));
    }

    [ObservableProperty] public partial ObservableCollection<IListProvider> ListProviders { get; set; }

    [ObservableProperty] public partial IListProvider SelectedListProvider { get; set; }

    [ObservableProperty] public partial ObservableCollection<WikiPageModel> Pages { get; set; } = [];

    [ObservableProperty] public partial ObservableCollection<WikiPageModel> SelectedPages { get; set; } = [];

    [ObservableProperty] public partial string NewPageTitle { get; set; } = string.Empty;

    [RelayCommand]
    private async Task AddNewPage()
    {
        if (!string.IsNullOrWhiteSpace(NewPageTitle))
        {
            Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(_settingsService.CurrentApiUrl, NewPageTitle);
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
        if (SelectedPages.Any())
        {
            Pages.Remove(new List<WikiPageModel>(SelectedPages));
        }

        SelectedPages = [];
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
            _ => throw new UnreachableException("Wait what? A list is either limited or unlimited.")
        };

        if (result is {IsSuccessful: true, Value: not null})
        {
            Pages.AddRange(result.Value);
        }
        else
        {
            await _dialogService.Alert("Failed to get the list",
                result.ErrorMessage ?? "Failed to get the list. Please make sure you have internet access.");
        }
    }

    [RelayCommand]
    private void OpenInBrowser()
    {
        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser(
                $"{_settingsService.GetCurrentSettings().GetIndexUrl()}title={HttpUtility.UrlEncode(selectedPage.Title)}");
        }
    }

    [RelayCommand]
    private void OpenHistoryInBrowser()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        foreach (WikiPageModel selectedPage in SelectedPages)
        {
            _systemService.OpenLinkInBrowser($"{_settingsService.GetCurrentSettings().GetIndexUrl()}title={selectedPage.Title}&action=history");
        }
    }

    [RelayCommand]
    private async Task Cut()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, SelectedPages.Select<WikiPageModel, string>(x => x.Title)));
        Pages.Remove([..SelectedPages]);
        SelectedPages = [];
    }

    [RelayCommand]
    private async Task Copy()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, SelectedPages.Select<WikiPageModel, string>(x => x.Title)));
    }

    [RelayCommand]
    private async Task Paste()
    {
        string? clipboardText = await _systemService.GetClipboardTextAsync();
        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            return;
        }

        string[] titles = clipboardText.Split([Environment.NewLine],
            StringSplitOptions.None);
        string urlApi = _settingsService.CurrentApiUrl;
        foreach (string title in titles)
        {
            Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(urlApi, title);
            if (result is {IsSuccessful: true, Value: not null})
            {
                Pages.Add(result.Value);
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
        SelectedPages = [];
    }

    [RelayCommand]
    private void SelectInverse()
    {
        var newSelection = Pages.Where(page => !SelectedPages.Contains(page)).ToList();

        SelectedPages = newSelection.ToObservableCollection();
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        Pages.Remove([..SelectedPages]);
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
        Pages = Pages.Distinct().ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void RemoveNonMainSpace()
    {
        Pages = Pages.Where(p => p.NamespaceId == 0).ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void MoveToTop()
    {
        var selectedPages = SelectedPages.ToList();
        Pages.Remove(selectedPages);
        Pages = selectedPages.Concat(Pages).ToObservableCollection();
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void MoveToBottom()
    {
        var selectedPages = SelectedPages.ToList();
        Pages.Remove(selectedPages);
        Pages.AddRange(selectedPages);
        SelectedPages.Clear();
    }

    [RelayCommand]
    private void ConvertToTalkPages()
    {
        List<WikiPageModel>? talkPages = _pageService.ConvertToTalk([..Pages]).Value;
        if (talkPages is not null)
        {
            Pages = talkPages.ToObservableCollection();
        }
    }

    [RelayCommand]
    private void ConvertFromTalkPages()
    {
        List<WikiPageModel>? subjectPages = _pageService.ConvertToSubject([..Pages]).Value;
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
            FilterOptions? filter = await _dialogService.ShowDialog<FilterOptions>(await _viewModelFactory.GetFilterViewModel());
            if (filter != null)
            {
                var filteredPages = Pages.ToList();
                filteredPages = filter.PerRemoveDuplicates(filteredPages);
                filteredPages = filter.PerNamespacesToKeep(filteredPages);
                filteredPages = filter.PerRemoveTitlesContaining(filteredPages);
                filteredPages = filter.PerKeepTitlesContaining(filteredPages);
                filteredPages = filter.PerSetOperation(filteredPages);
                filteredPages = filter.PerSortAlphabetically(filteredPages);
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
        if (Pages.Count == 0)
        {
            return;
        }

        string suggestedTitle = SelectedListProvider.Title;
        if (!string.IsNullOrWhiteSpace(SelectedListProvider.Param))
        {
            suggestedTitle += $"_{SelectedListProvider.Param}";
        }

        suggestedTitle += $"_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.txt";
        (_, Func<Task<Stream>>? openWriteStream) =
            await _fileDialogService.SaveFilePickerAsync("Save pages", suggestedFileName: suggestedTitle.ToFilenameSafe());
        if (openWriteStream is not null)
        {
            Stream stream = await openWriteStream();
            foreach ((string title, int _) in Pages)
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes($"# [[:{title}]]{Environment.NewLine}"));
            }

            stream.Close();
        }
    }

    [RelayCommand]
    private void SortAlphabetically()
    {
        Pages = Pages.OrderBy<WikiPageModel, string>(x => x.Title).ToObservableCollection();
    }

    [RelayCommand]
    private void SortReverseAlphabetically()
    {
        Pages = Pages.OrderByDescending<WikiPageModel, string>(x => x.Title).ToObservableCollection();
    }
}