using System;
using System.Text.RegularExpressions;
using System.Web;

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

    public static string? GetTitleFromURL(string link, Regex extractTitle)
    {
        link = extractTitle.Match(link).Groups[1].Value;

        return string.IsNullOrEmpty(link) ? null : WikiDecode(link);
    }

    /// <summary>
    /// Decodes URL-encoded page titles into a normal string
    /// </summary>
    /// <param name="title">Page title to decode</param>
    public static string WikiDecode(string title)
    {
        return HttpUtility.UrlDecode(title.Replace("+", "%2B")).Replace('_', ' ');
    }

    /// <summary>
    /// Returns index of first character different between strings
    /// </summary>
    /// <param name="a">First string</param>
    /// <param name="b">Second string</param>
    public static int FirstDifference(string a, string b)
    {
        for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
        {
            if (a[i] != b[i])
            {
                return i;
            }
        }

        return Math.Min(a.Length, b.Length);
    }

    [GeneratedRegex("\\[\\[:?([^\\|[\\]]+)(?:\\]\\]|\\|)", RegexOptions.Compiled)]
    public static partial Regex WikiLinkRegex();

    [GeneratedRegex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled)]
    public static partial Regex FromFileRegex();
}