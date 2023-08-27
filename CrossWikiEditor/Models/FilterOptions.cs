using System.Collections.Generic;

namespace CrossWikiEditor.Models;

public enum SetOperations
{
    SymmetricDifference,
    Intersection
}

public record FilterOptions(
    int[] NamespacesToKeep,
    string RemoveTitlesContaining,
    string KeepTitlesContaining,
    bool UseRegex,
    bool SortAlphabetically,
    bool RemoveDuplicates,
    SetOperations SetOperation,
    List<WikiPageModel> Pages);