namespace CrossWikiEditor.Core.Models;

public enum RedirectFilter
{
    All,
    Redirects,
    NoRedirects
}

public record NamespacesAndRedirectFilterOptions
{
    public NamespacesAndRedirectFilterOptions(int[] Namespaces, bool IncludeRedirects, RedirectFilter RedirectFilter)
    {
        this.Namespaces = Namespaces;
        this.IncludeRedirects = IncludeRedirects;
        this.RedirectFilter = RedirectFilter;
    }

    public int[] Namespaces { get; init; }
    public bool IncludeRedirects { get; init; }
    public RedirectFilter RedirectFilter { get; init; }

    public void Deconstruct(out int[] Namespaces, out bool IncludeRedirects, out RedirectFilter RedirectFilter)
    {
        Namespaces = this.Namespaces;
        IncludeRedirects = this.IncludeRedirects;
        RedirectFilter = this.RedirectFilter;
    }
}