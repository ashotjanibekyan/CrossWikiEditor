using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace CrossWikiEditor.Core.Utils;

public static partial class Tools
{
    /// <summary>
    ///     Removes underscores and wiki syntax from links
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

        if (text[0] is '#' or '*')
        {
            text = text[1..];
        }

        text = text.Replace("_", " ").Trim();
        text = text.Trim('[', ']');
        text = text.Replace("&amp;", "&");
        text = text.Replace("&quot;", @"""");
        text = text.Replace("�", "");

        return text.TrimStart(':');
    }

    /// <summary>
    ///     Returns index of first character different between strings
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

    public static string GetPageTitleFromUrl(string url)
    {
        var uri = new Uri(url);

        if (uri.Segments is [_, "w/", ..])
        {
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);
            string? pageTitle = queryParameters["title"];

            if (!string.IsNullOrEmpty(pageTitle))
            {
                pageTitle = UnescapeDataStringRec(pageTitle);
                return pageTitle.Replace("_", " ");
            }
        }

        // If the URL structure is not as expected, attempt to extract from the path
        string path = UnescapeDataStringRec(uri.AbsolutePath);
        string? extractedTitle = path[(path.LastIndexOf('/') + 1)..];

        return extractedTitle.Replace("_", " ");
    }

    private static string UnescapeDataStringRec(string url)
    {
        string result = url;
        string unescapedResult = Uri.UnescapeDataString(result);

        while (result != unescapedResult)
        {
            result = unescapedResult;
            unescapedResult = Uri.UnescapeDataString(result);
        }

        return result;
    }

    [GeneratedRegex(@"\[\[:?([^\|[\]]+)(?:\]\]|\|)", RegexOptions.Compiled)]
    public static partial Regex WikiLinkRegex();

    [GeneratedRegex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled)]
    public static partial Regex FromFileRegex();
}