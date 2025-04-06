using CommunityToolkit.Mvvm.ComponentModel;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class MoreViewModel : ViewModelBase
{
    private MoreOptions _moreOptions;

    public MoreViewModel(
        ISettingsService settingsService,
        IMessengerWrapper messenger)
    {
        messenger.Register<CurrentSettingsUpdatedMessage>(this, (r, m) =>
        {
            _moreOptions = settingsService.GetCurrentSettings().MoreOptions;
            PopulateProperties();
        });
        _moreOptions = settingsService.GetCurrentSettings().MoreOptions;
        PopulateProperties();
    }


    [ObservableProperty] public partial bool IsAppendOrPrependEnabled { get; set; }
    partial void OnIsAppendOrPrependEnabledChanged(bool value) => _moreOptions.IsAppendPrependEnabled = value;
    
    [ObservableProperty] public partial bool IsAppend { get; set; }
    partial void OnIsAppendChanged(bool value) => _moreOptions.IsAppend = value;
    
    [ObservableProperty] public partial string AppendOrPrependContent { get; set; } = string.Empty;
    partial void OnAppendOrPrependContentChanged(string value) => _moreOptions.AppendOrPrependContent = value;
    
    [ObservableProperty] public partial int AppendOrPrependNewLines { get; set; }
    partial void OnAppendOrPrependNewLinesChanged(int value) => _moreOptions.AppendOrPrependNewLines = value;
    
    [ObservableProperty] public partial bool ShouldSortMetadataAfterAppendOrPrepend { get; set; }
    partial void OnShouldSortMetadataAfterAppendOrPrependChanged(bool value) => _moreOptions.ShouldSortMetaDataAfterAppendOrPrepend = value;

    [ObservableProperty] public partial FileTaskType FileType { get; set; }
    partial void OnFileTypeChanged(FileTaskType value) => _moreOptions.FileActions[0].Type = value;
    
    [ObservableProperty] public partial string SourceFile { get; set; } = string.Empty;
    partial void OnSourceFileChanged(string value) => _moreOptions.FileActions[0].SourceFile = value;
    
    [ObservableProperty] public partial string ReplaceFileOrComment { get; set; } = string.Empty;
    partial void OnReplaceFileOrCommentChanged(string value) => _moreOptions.FileActions[0].ReplaceFileOrComment = value;
    
    [ObservableProperty] public partial bool SkipIfNoFileChanged { get; set; }
    partial void OnSkipIfNoFileChangedChanged(bool value) => _moreOptions.FileActions[0].SkipIfNoChanged = value;

    [ObservableProperty] public partial CategoryTaskType CategoryType { get; set; }
    partial void OnCategoryTypeChanged(CategoryTaskType value) => _moreOptions.CategoryActions[0].Type = value;
    
    [ObservableProperty] public partial string SourceCategory { get; set; } = string.Empty;
    partial void OnSourceCategoryChanged(string value) => _moreOptions.CategoryActions[0].SourceCategory = value;
    
    [ObservableProperty] public partial string ReplaceCategory { get; set; } = string.Empty;
    partial void OnReplaceCategoryChanged(string value) => _moreOptions.CategoryActions[0].ReplaceCategory = value;
    
    [ObservableProperty] public partial bool SkipIfNoCategoryChanged { get; set; }
    partial void OnSkipIfNoCategoryChangedChanged(bool value) => _moreOptions.CategoryActions[0].SkipIfNoChanged = value;
    
    [ObservableProperty] public partial bool RemoveSortkey { get; set; }
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