namespace CrossWikiEditor.Core.Settings;

public partial class NormalFindAndReplaceRule(
    string find,
    string replaceWith,
    bool caseSensitive,
    bool regex,
    bool multiLine,
    bool singleLine,
    bool minor,
    bool afterFixes,
    bool enabled,
    string comment) : ObservableObject
{
    public NormalFindAndReplaceRule() : this("", "", false, false, false, false, false, false, false, "")
    {
    }

    [ObservableProperty] public partial string Find { get; set; } = find;

    [ObservableProperty] public partial string ReplaceWith { get; set; } = replaceWith;

    [ObservableProperty] public partial bool CaseSensitive { get; set; } = caseSensitive;

    [ObservableProperty] public partial bool Regex { get; set; } = regex;

    [ObservableProperty] public partial bool MultiLine { get; set; } = multiLine;

    [ObservableProperty] public partial bool SingleLine { get; set; } = singleLine;

    [ObservableProperty] public partial bool Minor { get; set; } = minor;

    [ObservableProperty] public partial bool AfterFixes { get; set; } = afterFixes;

    [ObservableProperty] public partial bool Enabled { get; set; } = enabled;

    [ObservableProperty] public partial string Comment { get; set; } = comment;

    public bool IsEmpty => string.IsNullOrEmpty(Find);
}