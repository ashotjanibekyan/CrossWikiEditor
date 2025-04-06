using System.Collections.Generic;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;

namespace CrossWikiEditor.Core.Settings;

/// <summary>
///     Corresponds to the <see cref="MoreViewModel" />
/// </summary>
public sealed class MoreOptions
{
    public bool IsAppendPrependEnabled { get; set; }
    public bool IsAppend { get; set; }
    public string AppendOrPrependContent { get; set; } = string.Empty;
    public int AppendOrPrependNewLines { get; set; }
    public bool ShouldSortMetaDataAfterAppendOrPrepend { get; set; }
    public List<FileTask> FileActions { get; set; } = [new(FileTaskType.None)];
    public List<CategoryTask> CategoryActions { get; set; } = [new(CategoryTaskType.None)];
}

public enum FileTaskType
{
    None,
    Replace,
    Remove,
    CommentOut
}

public enum CategoryTaskType
{
    None,
    Replace,
    Add,
    Remove
}

public sealed class FileTask
{
    public FileTask(FileTaskType type)
    {
        Type = type;
    }

    public FileTaskType Type { get; set; }
    public string SourceFile { get; set; } = string.Empty;
    public string ReplaceFileOrComment { get; set; } = string.Empty;
    public bool SkipIfNoChanged { get; set; }
}

public sealed class CategoryTask
{
    public CategoryTask(CategoryTaskType type)
    {
        Type = type;
    }

    public CategoryTaskType Type { get; set; }
    public string SourceCategory { get; set; } = string.Empty;
    public string ReplaceCategory { get; set; } = string.Empty;
    public bool SkipIfNoChanged { get; set; }
    public bool RemoveSortkey { get; set; }
}