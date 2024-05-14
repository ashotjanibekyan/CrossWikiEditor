namespace CrossWikiEditor.Core.Utils;

public static class WikiPageModelExtensions
{
    public static string ToWikiList(this IEnumerable<WikiPageModel> wikiPageModels, bool isNumericList, int pagesPerSection)
    {
        string seperator = isNumericList ? "#" : "*";
        IEnumerable<WikiPageModel[]> pages = wikiPageModels.Chunk(pagesPerSection);
        var sb = new StringBuilder();
        int i = 1;
        foreach (WikiPageModel[] section in pages)
        {
            sb.Append($"== {i} =={Environment.NewLine}");
            foreach (WikiPageModel page in section)
            {
                sb.Append($"{seperator} [[{page.Title}]]{Environment.NewLine}");
            }
            i++;
        }
        return sb.ToString();
    }

    public static string ToWikiListAlphabetically(this IEnumerable<WikiPageModel> wikiPageModels, bool isNumericList)
    {
        string seperator = isNumericList ? "#" : "*";
        var sb = new StringBuilder();
        IEnumerable<IEnumerable<WikiPageModel>> pages = wikiPageModels.GroupBy(p => char.ToLower(p.Title[0])).OrderBy(p => p.First().Title).Select(l => l.OrderBy(p => p.Title));
        foreach (IEnumerable<WikiPageModel> section in pages)
        {
            sb.Append($"== {char.ToUpper(section.First().Title[0])} =={Environment.NewLine}");
            foreach (WikiPageModel page in section)
            {
                sb.Append($"{seperator} [[{page.Title}]]{Environment.NewLine}");
            }
        }
        return sb.ToString();
    }
}