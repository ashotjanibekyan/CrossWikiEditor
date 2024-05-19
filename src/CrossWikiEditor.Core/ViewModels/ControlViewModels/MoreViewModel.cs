namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class MoreViewModel : ViewModelBase
{
    private MoreOptions _moreOptions;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;

    public MoreViewModel(
        ISettingsService settingsService,
        IDialogService dialogService,
        IMessengerWrapper messenger)
    {
        messenger.Register<CurrentSettingsUpdatedMessage>(this, (r, m) =>
        {
            _moreOptions = settingsService.GetCurrentSettings().MoreOptions;
            PopulateProperties();
        });
        _dialogService = dialogService;
        _settingsService = settingsService;
        _moreOptions = _settingsService.GetCurrentSettings().MoreOptions;
        PopulateProperties();
    }

    [ObservableProperty] private bool _isAppendOrPrependEnabled = false;
    [ObservableProperty] private bool _isAppend = true;
    [ObservableProperty] private string _appendOrPrependContent = string.Empty;
    [ObservableProperty] private int _appendOrPrependNewLines = 0;
    [ObservableProperty] private bool _shouldSortMetadataAfterAppendOrPrepend;

    [ObservableProperty] private FileTaskType _fileType = FileTaskType.None;
    [ObservableProperty] private string _sourceFile = string.Empty;
    [ObservableProperty] private string _replaceFileOrComment = string.Empty;
    [ObservableProperty] private bool _skipIfNoFileChanged;

    [ObservableProperty] private CategoryTaskType _categoryType = CategoryTaskType.None;
    [ObservableProperty] private string _sourceCategory = string.Empty;
    [ObservableProperty] private string _replaceCategory = string.Empty;
    [ObservableProperty] private bool _skipIfNoCategoryChanged;
    [ObservableProperty] private bool _removeSortkey;

    partial void OnIsAppendOrPrependEnabledChanged(bool value) => _moreOptions.IsAppendPrependEnabled = value;
    partial void OnIsAppendChanged(bool value) => _moreOptions.IsAppend = value;
    partial void OnAppendOrPrependContentChanged(string value) => _moreOptions.AppendOrPrependContent = value;
    partial void OnAppendOrPrependNewLinesChanged(int value) => _moreOptions.AppendOrPrependNewLines = value;
    partial void OnShouldSortMetadataAfterAppendOrPrependChanged(bool value) => _moreOptions.ShouldSortMetaDataAfterAppendOrPrepend = value;
    partial void OnFileTypeChanged(FileTaskType value) => _moreOptions.FileActions[0].Type = value;
    partial void OnSourceFileChanged(string value) => _moreOptions.FileActions[0].SourceFile = value;
    partial void OnReplaceFileOrCommentChanged(string value) => _moreOptions.FileActions[0].ReplaceFileOrComment = value;
    partial void OnSkipIfNoFileChangedChanged(bool value) => _moreOptions.FileActions[0].SkipIfNoChanged = value;
    partial void OnCategoryTypeChanged(CategoryTaskType value) => _moreOptions.CategoryActions[0].Type = value;
    partial void OnSourceCategoryChanged(string value) => _moreOptions.CategoryActions[0].SourceCategory = value;
    partial void OnReplaceCategoryChanged(string value) => _moreOptions.CategoryActions[0].ReplaceCategory = value;
    partial void OnSkipIfNoCategoryChangedChanged(bool value) => _moreOptions.CategoryActions[0].SkipIfNoChanged = value;
    partial void OnRemoveSortkeyChanged(bool value) => _moreOptions.CategoryActions[0].RemoveSortkey = value;

    private void PopulateProperties()
    {
        IsAppendOrPrependEnabled = _moreOptions!.IsAppendPrependEnabled;
        IsAppend = _moreOptions.IsAppend;
        AppendOrPrependContent = _moreOptions.AppendOrPrependContent;
        AppendOrPrependNewLines = _moreOptions.AppendOrPrependNewLines;

        if (_moreOptions.FileActions.Count != 0)
        {
            FileType = _moreOptions.FileActions[0].Type;
            SourceFile = _moreOptions.FileActions[0].SourceFile;
            ReplaceFileOrComment = _moreOptions.FileActions[0].ReplaceFileOrComment;
            SkipIfNoFileChanged = _moreOptions.FileActions[0].SkipIfNoChanged;
        }

        if (_moreOptions.CategoryActions.Count != 0)
        {
            CategoryType = _moreOptions.CategoryActions[0].Type;
            SourceCategory = _moreOptions.CategoryActions[0].SourceCategory;
            ReplaceCategory = _moreOptions.CategoryActions[0].ReplaceCategory;
            SkipIfNoCategoryChanged = _moreOptions.CategoryActions[0].SkipIfNoChanged;
            RemoveSortkey = _moreOptions.CategoryActions[0].RemoveSortkey;
        }
    }
}