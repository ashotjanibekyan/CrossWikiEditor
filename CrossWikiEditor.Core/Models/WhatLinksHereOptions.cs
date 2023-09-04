namespace CrossWikiEditor.Core.Models;

public enum RedirectFilter
{
    All,
    Redirects,
    NoRedirects
}

public record WhatLinksHereOptions(int[] Namespaces, bool IncludeRedirects, RedirectFilter RedirectFilter);