namespace CrossWikiEditor.Core.ViewModels;

public partial class FilterViewModel : ViewModelBase
{
    private readonly TextFileListProvider _textFileListProvider;

    public FilterViewModel(
        List<WikiNamespace> subjectNamespaces,
        List<WikiNamespace> talkNamespaces,
        TextFileListProvider textFileListProvider)
    {
        _textFileListProvider = textFileListProvider;
        SubjectNamespaces = subjectNamespaces.ToObservableCollection();
        TalkNamespaces = talkNamespaces.ToObservableCollection();
        Pages = [];
        SetOperations = new[] {Models.SetOperations.SymmetricDifference, Models.SetOperations.Intersection}.ToObservableCollection();
        RemoveTitlesContaining = string.Empty;
        KeepTitlesContaining = string.Empty;
        SelectedSetOperations = Models.SetOperations.SymmetricDifference;
    }

    [ObservableProperty] public partial ObservableCollection<WikiNamespace> SubjectNamespaces { get; set; }
    [ObservableProperty] public partial ObservableCollection<WikiNamespace> TalkNamespaces { get; set; }
    [ObservableProperty] public partial ObservableCollection<WikiPageModel> Pages { get; set; }
    [ObservableProperty] public partial ObservableCollection<SetOperations> SetOperations { get; set; }
    [ObservableProperty] public partial bool IsAllTalkChecked { get; set; }
    [ObservableProperty] public partial bool IsAllSubjectChecked { get; set; }
    [ObservableProperty] public partial bool UseRegex { get; set; }
    [ObservableProperty] public partial bool SortAlphabetically { get; set; }
    [ObservableProperty] public partial bool RemoveDuplicates { get; set; }
    [ObservableProperty] public partial string RemoveTitlesContaining { get; set; }
    [ObservableProperty] public partial string KeepTitlesContaining { get; set; }
    [ObservableProperty] public partial SetOperations SelectedSetOperations { get; set; }

    [RelayCommand]
    private void Save(IDialog dialog)
    {
        IEnumerable<int> arr1 = SubjectNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        IEnumerable<int> arr2 = TalkNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        dialog.Close(new FilterOptions(
            arr1.Concat(arr2).ToArray(),
            RemoveTitlesContaining,
            KeepTitlesContaining,
            UseRegex,
            SortAlphabetically,
            RemoveDuplicates,
            SelectedSetOperations,
            [..Pages]));
    }

    [RelayCommand]
    private void Close(IDialog dialog)
    {
        dialog.Close(null);
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        await _textFileListProvider.GetAdditionalParams();

        if (_textFileListProvider.CanMake)
        {
            Result<List<WikiPageModel>> result = await _textFileListProvider.MakeList();
            if (result is {IsSuccessful: true, Value: not null})
            {
                Pages = result.Value.ToObservableCollection();
            }
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Pages.Clear();
    }

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
}