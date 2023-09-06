using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class DatabaseScannerViewModel(
    IDialogService dialogService,
    WikiSite wikiSite,
    IFileDialogService fileDialogService) : ViewModelBase
{
    private Task? _scannerTask;
    private Task? _updateUiTask;
    private CancellationTokenSource _scannerCancellationTokenSource = new();
    private ConcurrentQueue<string> _titlesQueue = new();
    
    [ObservableProperty] private ObservableCollection<WikiNamespace> _subjectNamespaces = new();
    [ObservableProperty] private ObservableCollection<WikiNamespace> _talkNamespaces = new();
    [ObservableProperty] private ObservableCollection<WikiPageModel> _pages = new();

    [ObservableProperty] private bool _isAllTalkChecked;
    [ObservableProperty] private bool _isAllSubjectChecked;
    [ObservableProperty] private string _databaseFile = string.Empty;
    [ObservableProperty] private string _siteName = string.Empty;
    [ObservableProperty] private string _base = string.Empty;
    [ObservableProperty] private string _generator = string.Empty;
    [ObservableProperty] private string _case = string.Empty;

    partial void OnIsAllTalkCheckedChanged(bool value)
    {
        TalkNamespaces = TalkNamespaces
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }

    partial void OnIsAllSubjectCheckedChanged(bool value)
    {
        SubjectNamespaces = SubjectNamespaces
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }
    
    [RelayCommand]
    public async Task BrowseCommand()
    {
        string[]? result = await fileDialogService.OpenFilePickerAsync("Open Database dump", false);
        if (result is not {Length: 1})
        {
            return;
        }

        DatabaseFile = result[0];
        
        using var reader = new XmlTextReader(result[0]);
        int dataFound = 0;
        while (reader.Read())
        {
            if (reader.Name.Length == 0)
            {
                continue;
            }
            dataFound++;

            switch (reader.Name)
            {
                case "sitename":
                    SiteName = reader.ReadString();
                    break;
                case "base":
                    Base = reader.ReadString();
                    break;
                case "generator":
                    Generator = reader.ReadString();
                    break;
                case "case":
                    Case = reader.ReadString();
                    break;
                case "namespace":
                {
                    var ns = new WikiNamespace(int.Parse(reader.GetAttribute("key")!), reader.ReadString());
                    if (ns.Id < 0)
                    {
                        continue;
                    }
                    if (ns.Id.IsEven())
                    {
                        SubjectNamespaces.Add(ns);
                    }
                    else
                    {
                        TalkNamespaces.Add(ns);
                    }

                    break;
                }
            }

            if (dataFound > 100)
            {
                return;
            }
        }
    }

    [RelayCommand]
    private void Start()
    {
        if (string.IsNullOrWhiteSpace(DatabaseFile) || _scannerTask != null)
        {
            return;
        }
        _scannerTask = Task.Run(() =>
        {
            using var reader = new XmlTextReader(DatabaseFile);
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (_scannerCancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
                if (reader.Name == "title")
                {
                    _titlesQueue.Enqueue(reader.ReadString());
                    reader.ReadToFollowing("page");
                }
            }

            return Task.CompletedTask;
        }, _scannerCancellationTokenSource.Token);
        
        _updateUiTask = Task.Run(async () =>
        {
            while (_scannerTask != null)
            {
                UpdateUi();
                await Task.Delay(1000);
            }
        });
    }

    [RelayCommand]
    private void Pause()
    {
        _scannerCancellationTokenSource.Cancel();
        _scannerTask = null;
        _scannerCancellationTokenSource = new CancellationTokenSource();
    }

    private void UpdateUi()
    {
        while (!_titlesQueue.IsEmpty)
        {
            if (_titlesQueue.TryDequeue(out var title))
            {
                Pages.Add(new WikiPageModel(new WikiPage(wikiSite, title)));
            }
        }
    }
}