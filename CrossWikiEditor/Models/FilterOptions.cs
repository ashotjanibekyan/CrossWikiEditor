namespace CrossWikiEditor.Models;

public record FilterOptions(
    int[] NamespacesToKeep,
    string RemoveTitlesContaining,
    string KeepTitlesContaining,
    bool UseRegex,
    bool SortAlphabetically,
    bool RemoveDuplicates);