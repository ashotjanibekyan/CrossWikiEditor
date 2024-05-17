namespace CrossWikiEditor.Core.Settings;

/// <summary>
/// Corresponds to the <see cref="MoreViewModel"/>
/// </summary>
public sealed class MoreOptions
{
    public bool AppendPrependEnabled { get; set; }
    public bool IsAppend { get; set; }
    public string AppendOrPrependText { get; set; } = string.Empty;
    public int UseNNewlines { get; set; }
    public bool SortMetaDataAfterAppendOrPrepend { get; set; }
    public List<FilesOrCategoryAction> FileActions { get; set; } = [];
    public List<FilesOrCategoryAction> CategoryActions { get; set; } = [];
}

public enum ActionType
{
    Replace,
    Remove,
    Add,
    CommentOut
}
public sealed class FilesOrCategoryAction(ActionType actionType)
{
    public ActionType Type { get; set; } = actionType;
    public bool SkipIfNotChanged { get; set; }
    public string? Value1 { get; set; }
    public string? Value2 { get; set; }
}