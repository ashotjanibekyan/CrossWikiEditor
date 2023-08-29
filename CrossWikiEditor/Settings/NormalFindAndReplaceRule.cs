using CommunityToolkit.Mvvm.ComponentModel;

namespace CrossWikiEditor.Settings;

public partial class NormalFindAndReplaceRule(string find,
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

    [ObservableProperty] private string _find = find;
    [ObservableProperty] private string _replaceWith = replaceWith;
    [ObservableProperty] private bool _caseSensitive = caseSensitive;
    [ObservableProperty] private bool _regex = regex;
    [ObservableProperty] private bool _multiLine = multiLine;
    [ObservableProperty] private bool _singleLine = singleLine;
    [ObservableProperty] private bool _minor = minor;
    [ObservableProperty] private bool _afterFixes = afterFixes;
    [ObservableProperty] private bool _enabled = enabled;
    [ObservableProperty] private string _comment = comment;
    public bool IsEmpty => string.IsNullOrEmpty(Find);
}