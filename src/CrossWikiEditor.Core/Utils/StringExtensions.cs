namespace CrossWikiEditor.Core.Utils;

public static class StringExtensions
{
    public static string ToFilenameSafe(this string str)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        var result = new StringBuilder(str.Length);

        foreach (char c in str)
        {
            result.Append(Array.IndexOf(invalidChars, c) >= 0 ? '-' : c);
        }

        return result.ToString();
    }

    public static bool Contains(
        this string str,
        string value,
        bool isRegex,
        RegexOptions regexOptions = RegexOptions.Multiline)
    {
        if (!isRegex)
        {
            return str.Contains(value);
        }

        var r = new Regex(value, regexOptions);
        return r.IsMatch(str);
    }

    public static bool Contains(
        this string str,
        string value,
        bool isRegex,
        bool isCaseSensitive)
    {
        if (!isRegex)
        {
            return isCaseSensitive ? str.Contains(value) : str.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        Regex r = isCaseSensitive ? new Regex(value) : new Regex(value, RegexOptions.IgnoreCase);
        return r.IsMatch(str);
    }
}