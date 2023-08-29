using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ViewModels;

public sealed partial class MakeListViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IWikiClientCache _clientCache;
    private readonly IPageService _pageService;
    private readonly ISystemService _systemService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IFileDialogService _fileDialogService;
    private readonly IUserPreferencesService _userPreferencesService;

    public MakeListViewModel(
        IMessenger messenger,
        IDialogService dialogService,
        IWikiClientCache clientCache,
        IPageService pageService,
        ISystemService systemService,
        IViewModelFactory viewModelFactory,
        IFileDialogService fileDialogService,
        IUserPreferencesService userPreferencesService,
        IEnumerable<IListProvider> listProviders)
    {
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
            Pages.Add(result is { IsSuccessful: true, Value: not null } ? result.Value : new WikiPageModel(NewPageTitle.Trim(), 0));
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

        Result<List<WikiPageModel>> result = await SelectedListProvider.MakeList();
        if (result is { IsSuccessful: true, Value: not null })
        {
            Pages.AddRange(result.Value);
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
        if (!SelectedPages.Any())
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
        if (!SelectedPages.Any())
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
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, SelectedPages.Select(x => x.Title)));
        Pages.Remove(SelectedPages.ToList());
        SelectedPages = new ObservableCollection<WikiPageModel>();
    }

    [RelayCommand]
    private async Task Copy()
    {
        if (!SelectedPages.Any())
        {
            return;
        }

        await _systemService.SetClipboardTextAsync(string.Join(Environment.NewLine, SelectedPages.Select(x => x.Title)));
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
                Pages.Add(result is { IsSuccessful: true, Value: not null } ? result.Value : new WikiPageModel(title.Trim(), 0));
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
        var newSelection = Pages.Where(page => !SelectedPages.Contains(page)).ToList();

        SelectedPages = newSelection.ToObservableCollection();
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        Pages.Remove(SelectedPages.ToList());
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
    private async Task ConvertToTalkPages()
    {
        List<WikiPageModel>? talkPages = (await _pageService.ConvertToTalk(Pages.ToList())).Value;
        if (talkPages is not null)
        {
            Pages = talkPages.ToObservableCollection();
        }
    }

    [RelayCommand]
    private async Task ConvertFromTalkPages()
    {
        List<WikiPageModel>? subjectPages = (await _pageService.ConvertToSubject(Pages.ToList())).Value;
        if (subjectPages is not null)
        {
            Pages = subjectPages.ToObservableCollection();
        }
    }

    [RelayCommand]
    private void FormatPageTitlesPerDisplayTitle()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task Filter()
    {
        try
        {
            FilterOptions? result = await _dialogService.ShowDialog<FilterOptions>(await _viewModelFactory.GetFilterViewModel());
            if (result != null)
            {
                var filteredPages = Pages.Where(page => page.ShouldKeepPer(result)).ToList();

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
                    }

                    filteredPages = new List<WikiPageModel>(list);
                }

                Pages = filteredPages.ToObservableCollection();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [RelayCommand]
    private async Task SaveList()
    {
        string? suggestedTitle =
            $"{SelectedListProvider.Title}_{SelectedListProvider.Param}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.txt";
        IStorageFile? storageFile = await _fileDialogService.SaveFilePickerAsync("Save pages", suggestedFileName: suggestedTitle.ToFilenameSafe());
        if (storageFile is not null)
        {
            Stream stream = await storageFile.OpenWriteAsync();
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
        Pages = Pages.OrderBy(x => x.Title).ToObservableCollection();
    }

    [RelayCommand]
    private void SortReverseAlphabetically()
    {
        Pages = Pages.OrderByDescending(x => x.Title).ToObservableCollection();
    }

    [ObservableProperty] private ObservableCollection<IListProvider> _listProviders;
    [ObservableProperty] private IListProvider _selectedListProvider;
    [ObservableProperty] private ObservableCollection<WikiPageModel> _pages = new();
    [ObservableProperty] private ObservableCollection<WikiPageModel> _selectedPages = new();
    [ObservableProperty] private string _newPageTitle = string.Empty;
}