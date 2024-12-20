using System.Diagnostics;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class DatabaseScannerViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly ISettingsService _settingsService;
    private readonly ConcurrentQueue<string> _titlesQueue = new();
    private readonly IWikiClientCache _wikiClientCache;
    public EventHandler<string>? _convertedTextChanged;
    private CancellationTokenSource _scannerCancellationTokenSource = new();
    private Task? _scannerTask;
    private Task? _updateUiTask;

    public DatabaseScannerViewModel(
        ISettingsService settingsService,
        IWikiClientCache wikiClientCache,
        IFileDialogService fileDialogService)
    {
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
        _fileDialogService = fileDialogService;
        SubjectNamespaces = [];
        TalkNamespaces = [];
        Pages = [];
        TitleContains = string.Empty;
        TitleNotContains = string.Empty;
        DatabaseFile = string.Empty;
        SiteName = string.Empty;
        Base = string.Empty;
        Generator = string.Empty;
        Case = string.Empty;
        MinStartYear = new DateTimeOffset(new DateTime(2000, 1, 1));
        MinEndYear = new DateTimeOffset(new DateTime(2000, 1, 1));
        ConvertedText = string.Empty;
        NumberOfPagesOnEachSection = 25;
    }

    [ObservableProperty] public partial ObservableCollection<WikiNamespace> SubjectNamespaces { get; set; }
    [ObservableProperty] public partial ObservableCollection<WikiNamespace> TalkNamespaces { get; set; }
    [ObservableProperty] public partial ObservableCollection<WikiPageModel> Pages { get; set; }

    [ObservableProperty] public partial bool IsTitleContainsEnabled { get; set; }
    [ObservableProperty] public partial bool IsTitleNotContainsEnabled { get; set; }
    [ObservableProperty] public partial string TitleContains { get; set; }
    [ObservableProperty] public partial string TitleNotContains { get; set; }
    [ObservableProperty] public partial bool IsTitleContainsRegex { get; set; }
    [ObservableProperty] public partial bool IsTitleContainsCaseSensitive { get; set; }

    [ObservableProperty] public partial bool IsAllTalkChecked { get; set; }
    [ObservableProperty] public partial bool IsAllSubjectChecked { get; set; }
    [ObservableProperty] public partial string DatabaseFile { get; set; }
    [ObservableProperty] public partial string SiteName { get; set; }
    [ObservableProperty] public partial string Base { get; set; }
    [ObservableProperty] public partial string Generator { get; set; }
    [ObservableProperty] public partial string Case { get; set; }

    [ObservableProperty] public partial bool IsSearchDateChecked { get; set; }
    [ObservableProperty] public partial DateTimeOffset SelectedStartDate { get; set; }
    [ObservableProperty] public partial DateTimeOffset SelectedEndDate { get; set; }
    [ObservableProperty] public partial DateTimeOffset MinStartYear { get; set; }
    [ObservableProperty] public partial DateTimeOffset MinEndYear { get; set; }

    [ObservableProperty] public partial string ConvertedText { get; set; }
    [ObservableProperty] public partial bool IsAlphabetisedHeading { get; set; }
    [ObservableProperty] public partial int NumberOfPagesOnEachSection { get; set; }
    [ObservableProperty] public partial bool IsNumericList { get; set; }

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
        string[]? result = await _fileDialogService.OpenFilePickerAsync("Open Database dump", false);
        if (result is not {Length: 1})
        {
            return;
        }

        DatabaseFile = result[0];
        SetUpFromDb(DatabaseFile);
    }

    [RelayCommand]
    private void Start()
    {
        if (string.IsNullOrWhiteSpace(DatabaseFile) || _scannerTask != null)
        {
            return;
        }

        _scannerTask = Task.Run(Scan, _scannerCancellationTokenSource.Token);

        _updateUiTask = Task.Run(async () =>
        {
            WikiSite wikiSite = await _wikiClientCache.GetWikiSite(_settingsService.CurrentApiUrl);
            while (_scannerTask != null)
            {
                UpdateUi(wikiSite);
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

    [RelayCommand]
    private void Save(IDialog dialog)
    {
        _scannerTask = null;
        _scannerCancellationTokenSource.Cancel();
        dialog.Close(Pages.ToList());
    }

    [RelayCommand]
    private void Make()
    {
        if (Pages.Count == 0)
        {
            return;
        }

        if (IsAlphabetisedHeading)
        {
            MakeAlphabetisedList();
        }
        else
        {
            MakeNumericList();
        }
    }

    private void Scan()
    {
        using var reader = new XmlTextReader(DatabaseFile);
        while (reader.Read())
        {
            if (_scannerCancellationTokenSource.IsCancellationRequested)
            {
                break;
            }

            if (reader.NodeType != XmlNodeType.Element || reader.Name != "page")
            {
                continue;
            }

            DbPage page = ParsePageElement(reader);
            if (ShouldIncludePage(page))
            {
                _titlesQueue.Enqueue(page.Title!);
            }
        }

        _scannerTask = null;
    }

    private bool ShouldIncludePage(DbPage page)
    {
        if (ViolatesNamespaceCondition(page))
        {
            return false;
        }

        if (ViolatesTitleContains(page))
        {
            return false;
        }

        if (ViolatesTitleNotContains(page))
        {
            return false;
        }

        if (ViolatesRevisionDateRange(page))
        {
            return false;
        }

        return true;
    }

    private bool ViolatesNamespaceCondition(DbPage page)
    {
        var checkedNamespaces = SubjectNamespaces.Where(ns => ns.IsChecked).ToList();
        checkedNamespaces.AddRange(TalkNamespaces.Where(ns => ns.IsChecked));
        if (checkedNamespaces.Count == 0)
        {
            return false;
        }

        return checkedNamespaces.All(ns => ns.Id != page.Ns);
    }

    private bool ViolatesTitleContains(DbPage page)
    {
        if (!IsTitleContainsEnabled)
        {
            return false;
        }

        if (page.Revision.Count == 0)
        {
            return true;
        }

        string? currentText = page.Revision[^1].Text;
        if (currentText == null)
        {
            return true;
        }

        return !currentText.Contains(TitleContains, IsTitleContainsRegex, IsTitleContainsCaseSensitive);
    }

    private bool ViolatesTitleNotContains(DbPage page)
    {
        if (!IsTitleNotContainsEnabled)
        {
            return false;
        }

        string? currentText = page.Revision[^1].Text;
        return currentText?.Contains(TitleNotContains, IsTitleContainsRegex, IsTitleContainsCaseSensitive) == true;
    }

    private bool ViolatesRevisionDateRange(DbPage page)
    {
        return IsSearchDateChecked && page.Revision.All(rev => rev.Timestamp <= SelectedStartDate || rev.Timestamp >= SelectedEndDate);
    }

    private DbPage ParsePageElement(XmlReader reader)
    {
        string title = string.Empty;
        int ns = 0;
        int id = 0;
        List<DbRevision> revisions = [];
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "title":
                        title = reader.ReadElementContentAsString();
                        break;
                    case "ns":
                        ns = reader.ReadElementContentAsInt();
                        break;
                    case "id":
                        id = reader.ReadElementContentAsInt();
                        break;
                    case "revision":
                        revisions.Add(ParseRevisionElement(reader));
                        break;
                }
            }

            if (reader is {NodeType: XmlNodeType.EndElement, Name: "page"})
            {
                return new DbPage
                {
                    Id = id,
                    Ns = ns,
                    Revision = revisions,
                    Title = title
                };
            }
        }

        throw new UnreachableException();
    }

    private DbRevision ParseRevisionElement(XmlReader reader)
    {
        int id = -1;
        int parentId = -1;
        DateTime dateTime = default;
        DbContributor? contributor = default;
        string model = string.Empty;
        string format = string.Empty;
        string text = string.Empty;
        string comment = string.Empty;
        long textSize = 0;
        string sha1 = string.Empty;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "id":
                        id = reader.ReadElementContentAsInt();
                        break;
                    case "parentid":
                        parentId = reader.ReadElementContentAsInt();
                        break;
                    case "timestamp":
                        dateTime = reader.ReadElementContentAsDateTime();
                        break;
                    case "contributor":
                        contributor = ParseContributorElement(reader);
                        break;
                    case "comment":
                        comment = reader.ReadElementContentAsString();
                        break;
                    case "model":
                        model = reader.ReadElementContentAsString();
                        break;
                    case "format":
                        format = reader.ReadElementContentAsString();
                        break;
                    case "text":
                        textSize = long.Parse(reader.GetAttribute("bytes") ?? "0");
                        text = reader.ReadElementContentAsString();
                        break;
                    case "sha1":
                        sha1 = reader.ReadElementContentAsString();
                        break;
                }
            }

            if (reader is {NodeType: XmlNodeType.EndElement, Name: "revision"})
            {
                return new DbRevision
                {
                    Id = id,
                    Parentid = parentId,
                    Timestamp = dateTime,
                    Comment = comment,
                    Contributor = contributor,
                    Format = format,
                    Model = model,
                    Text = text,
                    TextSize = textSize,
                    Sha1 = sha1
                };
            }
        }

        throw new UnreachableException();
    }

    private DbContributor ParseContributorElement(XmlReader reader)
    {
        string username = string.Empty;
        int id = 0;
        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "id":
                    id = reader.ReadElementContentAsInt();
                    break;
                case "username":
                    username = reader.ReadElementContentAsString();
                    break;
            }

            if (reader is {NodeType: XmlNodeType.EndElement, Name: "contributor"})
            {
                return new DbContributor
                {
                    Id = id,
                    Username = username
                };
            }
        }

        throw new UnreachableException();
    }

    private void ParseSiteInfo(XmlReader reader)
    {
        while (reader.Read())
        {
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
                case "namespaces":
                {
                    ParseNamespaces(reader);
                    break;
                }
            }

            if (reader is {NodeType: XmlNodeType.EndElement, Name: "siteinfo"})
            {
                break;
            }
        }
    }

    private void ParseNamespaces(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name == "namespace")
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
            }

            if (reader is {NodeType: XmlNodeType.EndElement, Name: "namespaces"})
            {
                break;
            }
        }
    }

    private void SetUpFromDb(string dbPath)
    {
        using var reader = new XmlTextReader(dbPath);
        while (reader.Read())
        {
            if (reader.Name == "siteinfo")
            {
                ParseSiteInfo(reader);
                break;
            }
        }
    }

    private void MakeAlphabetisedList()
    {
        _convertedTextChanged?.Invoke(this, Pages.ToWikiListAlphabetically(IsNumericList));
    }

    private void MakeNumericList()
    {
        _convertedTextChanged?.Invoke(this, Pages.ToWikiList(IsNumericList, NumberOfPagesOnEachSection));
    }

    private void UpdateUi(WikiSite wikiSite)
    {
        while (!_titlesQueue.IsEmpty)
        {
            if (_titlesQueue.TryDequeue(out string? title))
            {
                Pages.Add(new WikiPageModel(new WikiPage(wikiSite, title)));
            }
        }
    }
}