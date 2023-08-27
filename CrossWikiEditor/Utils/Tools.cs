using System.Text.RegularExpressions;

namespace CrossWikiEditor.Utils;

public static partial class Tools
{
    private static readonly char[] InvalidChars = { '[', ']', '{', '}', '|', '<', '>', '#' };
    
    /// <summary>
    /// Removes underscores and wiki syntax from links
    /// </summary>
    public static string RemoveSyntax(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        text = text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        if (text[0] == '#' || text[0] == '*')
        {
            text = text[1..];
        }

        text = text.Replace("_", " ").Trim();
        text = text.Trim('[', ']');
        text = text.Replace(@"&amp;", @"&");
        text = text.Replace(@"&quot;", @"""");
        text = text.Replace(@"�", "");

        return text.TrimStart(':');
    }
    
    [GeneratedRegex("\\[\\[:?([^\\|[\\]]+)(?:\\]\\]|\\|)", RegexOptions.Compiled)]
    public static partial Regex WikiLinkRegex();
    
    [GeneratedRegex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled)]
    public static partial Regex FromFileRegex();
}