namespace CrossWikiEditor.Core.Models;

public enum SetOperations
{
    SymmetricDifference,
    Intersection
}

public class FilterOptions(
    IReadOnlyCollection<int> namespacesToKeep,
    string removeTitlesContaining,
    string keepTitlesContaining,
    bool useRegex,
    bool sortAlphabetically,
    bool removeDuplicates,
    SetOperations setOperation,
    IReadOnlyCollection<WikiPageModel> filterPages)
{
    public IReadOnlyCollection<int> NamespacesToKeep { get; } = namespacesToKeep;
    public string RemoveTitlesContaining { get; } = removeTitlesContaining;
    public string KeepTitlesContaining { get; } = keepTitlesContaining;
    public bool UseRegex { get; } = useRegex;
    public bool SortAlphabetically { get; } = sortAlphabetically;
    public bool RemoveDuplicates { get; } = removeDuplicates;
    public SetOperations SetOperation { get; } = setOperation;
    public IReadOnlyCollection<WikiPageModel> FilterPages { get; } = filterPages;

    public List<WikiPageModel> PerRemoveDuplicates(IEnumerable<WikiPageModel> pages)
    {
        return RemoveDuplicates ? pages.Distinct().ToList() : pages.ToList();
    }

    public List<WikiPageModel> PerNamespacesToKeep(IEnumerable<WikiPageModel> pages)
    {
        return NamespacesToKeep.Count > 0 ? pages.Where(p => NamespacesToKeep.Contains(p.NamespaceId)).ToList() : pages.ToList();
    }

    public List<WikiPageModel> PerRemoveTitlesContaining(IEnumerable<WikiPageModel> pages)
    {
        return RemoveTitlesContaining != string.Empty
            ? pages.Where(p => !p.Title.Contains(RemoveTitlesContaining, UseRegex)).ToList()
            : pages.ToList();
    }

    public List<WikiPageModel> PerKeepTitlesContaining(IEnumerable<WikiPageModel> pages)
    {
        return KeepTitlesContaining != string.Empty ? pages.Where(p => p.Title.Contains(KeepTitlesContaining, UseRegex)).ToList() : pages.ToList();
    }

    public List<WikiPageModel> PerSetOperation(List<WikiPageModel> pages)
    {
        if (FilterPages.Count == 0)
        {
            return pages;
        }

        var list = new HashSet<WikiPageModel>(pages);
        if (SetOperation == SetOperations.SymmetricDifference)
        {
            list.ExceptWith(FilterPages);
        }

        if (SetOperation == SetOperations.Intersection)
        {
            list.IntersectWith(FilterPages);
        }

        return new List<WikiPageModel>(list);
    }

    public List<WikiPageModel> PerSortAlphabetically(IEnumerable<WikiPageModel> pages)
    {
        return SortAlphabetically ? [..pages.OrderBy(p => p.Title)] : pages.ToList();
    }
}