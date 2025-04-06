using CommunityToolkit.Mvvm.ComponentModel;

namespace CrossWikiEditor.Core.Settings;

public partial class NormalFindAndReplaceRule : ObservableObject
{
    public NormalFindAndReplaceRule() : this("", "", false, false, false, false, false, false, false, "")
    {
    }

    public NormalFindAndReplaceRule(string find,
        string replaceWith,
        bool caseSensitive,
        bool regex,
        bool multiLine,
        bool singleLine,
        bool minor,
        bool afterFixes,
        bool enabled,
        string comment)
    {
        Find = find;
        ReplaceWith = replaceWith;
        CaseSensitive = caseSensitive;
        Regex = regex;
        MultiLine = multiLine;
        SingleLine = singleLine;
        Minor = minor;
        AfterFixes = afterFixes;
        Enabled = enabled;
        Comment = comment;
    }

    [ObservableProperty] public partial string Find { get; set; }

    [ObservableProperty] public partial string ReplaceWith { get; set; }

    [ObservableProperty] public partial bool CaseSensitive { get; set; }

    [ObservableProperty] public partial bool Regex { get; set; }

    [ObservableProperty] public partial bool MultiLine { get; set; }

    [ObservableProperty] public partial bool SingleLine { get; set; }

    [ObservableProperty] public partial bool Minor { get; set; }

    [ObservableProperty] public partial bool AfterFixes { get; set; }

    [ObservableProperty] public partial bool Enabled { get; set; }

    [ObservableProperty] public partial string Comment { get; set; }

    public bool IsEmpty => string.IsNullOrEmpty(Find);
}