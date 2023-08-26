using System;
using System.IO;
using System.Text;

namespace CrossWikiEditor.Utils;

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
}