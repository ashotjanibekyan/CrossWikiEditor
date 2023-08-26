using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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
    private readonly IPageService _pageService;
    private readonly ISystemService _systemService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IUserPreferencesService _userPreferencesService;

    public MakeListViewModel(IDialogService dialogService,
        IWikiClientCache clientCache,
        IPageService pageService,
        ISystemService systemService,
        IFileDialogService fileDialogService,
        IUserPreferencesService userPreferencesService,
        IEnumerable<IListProvider> listProviders)
    {
        _dialogService = dialogService;
        _clientCache = clientCache;
        _pageService = pageService;
        _systemService = systemService;
        _fileDialogService = fileDialogService;
        _userPreferencesService = userPreferencesService;
        AddNewPageCommand = ReactiveCommand.CreateFromTask(AddNewPage);
        RemoveCommand = ReactiveCommand.Create(Remove);
        MakeListCommand = ReactiveCommand.CreateFromTask(MakeList);
        OpenInBrowserCommand = ReactiveCommand.Create(OpenInBrowser);
        OpenHistoryInBrowserCommand = ReactiveCommand.Create(OpenHistoryInBrowser);
        CutCommand = ReactiveCommand.CreateFromTask(Cut);
        CopyCommand = ReactiveCommand.CreateFromTask(Copy);
        PasteCommand = ReactiveCommand.CreateFromTask(Paste);
        SelectAllCommand = ReactiveCommand.Create(SelectAll);
        SelectNoneCommand = ReactiveCommand.Create(SelectNone);
        SelectInverseCommand = ReactiveCommand.Create(SelectInverse);
        RemoveSelectedCommand = ReactiveCommand.Create(RemoveSelected);
        RemoveAllCommand = ReactiveCommand.Create(RemoveAll);
        RemoveDuplicateCommand = ReactiveCommand.Create(RemoveDuplicate);
        RemoveNonMainSpaceCommand = ReactiveCommand.Create(RemoveNonMainSpace);
        MoveToTopCommand = ReactiveCommand.Create(MoveToTop);
        MoveToBottomCommand = ReactiveCommand.Create(MoveToBottom);
        ConvertToTalkPagesCommand = ReactiveCommand.CreateFromTask(ConvertToTalkPages);
        ConvertFromTalkPagesCommand = ReactiveCommand.CreateFromTask(ConvertFromTalkPages);
        FormatPageTitlesPerDisplayTitleCommand = ReactiveCommand.Create(FormatPageTitlesPerDisplayTitle);
        FilterCommand = ReactiveCommand.Create(Filter);
        SaveListCommand = ReactiveCommand.CreateFromTask(SaveList);
        SortAlphabeticallyCommand = ReactiveCommand.Create(SortAlphabetically);
        SortReverseAlphabeticallyCommand = ReactiveCommand.Create(SortReverseAlphabetically);
        
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

    private async Task Paste()
    {
        string? clipboardText = await _systemService.GetClipboardTextAsync();
        if (!string.IsNullOrWhiteSpace(clipboardText))
        {
            string[] titles = clipboardText.Split(new[] {Environment.NewLine},
                StringSplitOptions.None);
            string urlApi = _userPreferencesService.GetCurrentPref().UrlApi();
            foreach (string title in titles)
            {
                Result<WikiPageModel> result = await _clientCache.GetWikiPageModel(urlApi, title);
                Pages.Add(result is {IsSuccessful: true, Value: not null} ? result.Value : new WikiPageModel(title.Trim()));
            }
        }
    }

    private void SelectAll()
    {
        foreach (WikiPageModel page in Pages)
        {
            SelectedPages.Add(page);
        }
    }

    private void SelectNone()
    {
        SelectedPages = new ObservableCollection<WikiPageModel>();
    }
    
    private void SelectInverse()
    {
        var newSelection = Pages.Where(page => !SelectedPages.Contains(page)).ToList();

        SelectedPages = newSelection.ToObservableCollection();
    }

    private void RemoveSelected()
    {
        Pages.Remove(SelectedPages);
        SelectedPages.Clear();
    }
    
    private void RemoveAll()
    {
        Pages.Clear();
        SelectedPages.Clear();
    }

    private void RemoveDuplicate()
    {
        Pages = Pages.Distinct().ToObservableCollection();
        SelectedPages.Clear();
    }

    private void RemoveNonMainSpace()
    {
        Pages = Pages.Where(p => p.NamespaceId == 0).ToObservableCollection();
        SelectedPages.Clear();
    }
    
    private void MoveToTop()
    {
        var selectedPages = SelectedPages.ToList();
        Pages.Remove(selectedPages);
        Pages = selectedPages.Concat(Pages).ToObservableCollection();
        SelectedPages.Clear();
    }
    
    private void MoveToBottom()
    {
        var selectedPages = SelectedPages.ToList();
        Pages.Remove(selectedPages);
        Pages.AddRange(selectedPages);
        SelectedPages.Clear();
    }
    
    private async Task ConvertToTalkPages()
    {
        List<WikiPageModel>? talkPages = (await _pageService.ConvertToTalk(Pages.ToList())).Value;
        if (talkPages is not null)
        {
            Pages = talkPages.ToObservableCollection();
        }
    }

    private async Task ConvertFromTalkPages()
    {
        List<WikiPageModel>? subjectPages = (await _pageService.ConvertToSubject(Pages.ToList())).Value;
        if (subjectPages is not null)
        {
            Pages = subjectPages.ToObservableCollection();
        }
    }

    private void FormatPageTitlesPerDisplayTitle()
    {
        throw new NotImplementedException();
    }

    private void Filter()
    {
        throw new NotImplementedException();
    }

    private async Task SaveList()
    {
        var suggestedTitle =
            $"{SelectedListProvider.Title}_{SelectedListProvider.Param}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.txt";
        IStorageFile? storageFile = await _fileDialogService.SaveFilePickerAsync("Save pages", suggestedFileName: suggestedTitle.ToFilenameSafe());
        if (storageFile is not null)
        {
            Stream stream = await storageFile.OpenWriteAsync();
            foreach ((string title, int _) in Pages)
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes($"# [[:{title}]]\n"));
            }
            stream.Close();
        }
    }

    private void SortAlphabetically()
    {
        Pages = Pages.OrderBy(x => x.Title).ToObservableCollection();
    }

    private void SortReverseAlphabetically()
    {
        Pages = Pages.OrderByDescending(x => x.Title).ToObservableCollection();
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
    public ReactiveCommand<Unit, Unit> RemoveSelectedCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveAllCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveDuplicateCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveNonMainSpaceCommand { get; }
    public ReactiveCommand<Unit, Unit> MoveToTopCommand { get; }
    public ReactiveCommand<Unit, Unit> MoveToBottomCommand { get; }
    public ReactiveCommand<Unit, Unit> ConvertToTalkPagesCommand { get; }
    public ReactiveCommand<Unit, Unit> ConvertFromTalkPagesCommand { get; }
    public ReactiveCommand<Unit, Unit> FormatPageTitlesPerDisplayTitleCommand { get; }
    public ReactiveCommand<Unit, Unit> FilterCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveListCommand { get; }
    public ReactiveCommand<Unit, Unit> SortAlphabeticallyCommand { get; }
    public ReactiveCommand<Unit, Unit> SortReverseAlphabeticallyCommand { get; }
}