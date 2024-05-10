namespace CrossWikiEditor.Core.Models;

public enum RedirectFilter
{
    All,
    Redirects,
    NoRedirects
}

public record NamespacesAndRedirectFilterOptions(int[] Namespaces, bool IncludeRedirects, RedirectFilter RedirectFilter);