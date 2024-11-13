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
        IsAppendOrPrependEnabled = false;
        IsAppend = true;
        AppendOrPrependContent = string.Empty;
        FileType = FileTaskType.None;
        SourceFile = string.Empty;
        ReplaceFileOrComment = string.Empty;
        CategoryType = CategoryTaskType.None;
        SourceCategory = string.Empty;
        ReplaceCategory = string.Empty;
        PopulateProperties();
    }


    [ObservableProperty] public partial bool IsAppendOrPrependEnabled { get; set; }
    [ObservableProperty] public partial bool IsAppend { get; set; }
    [ObservableProperty] public partial string AppendOrPrependContent { get; set; }
    [ObservableProperty] public partial int AppendOrPrependNewLines { get; set; }
    [ObservableProperty] public partial bool ShouldSortMetadataAfterAppendOrPrepend { get; set; }

    [ObservableProperty] public partial FileTaskType FileType { get; set; }
    [ObservableProperty] public partial string SourceFile { get; set; }
    [ObservableProperty] public partial string ReplaceFileOrComment { get; set; }
    [ObservableProperty] public partial bool SkipIfNoFileChanged { get; set; }

    [ObservableProperty] public partial CategoryTaskType CategoryType { get; set; }
    [ObservableProperty] public partial string SourceCategory { get; set; }
    [ObservableProperty] public partial string ReplaceCategory { get; set; }
    [ObservableProperty] public partial bool SkipIfNoCategoryChanged { get; set; }
    [ObservableProperty] public partial bool RemoveSortkey { get; set; }

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