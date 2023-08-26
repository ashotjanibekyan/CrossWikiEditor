using System.Collections.Generic;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Generators;

public class FilesGenerator : WikiPagePropertyGenerator
{
    public FilesGenerator(WikiSite site, string pageTitle) : base(site)
    {
        PageTitle = pageTitle;
    }

    public override string PropertyName => "images";
    public IEnumerable<string>? MatchingTitles { get; set; }
    public bool OrderDescending { get; set; }
    
    public override IEnumerable<KeyValuePair<string, object>> EnumListParameters()
    {
        var dict = new Dictionary<string, object>
        {
            {"imlimit", PaginationSize}, 
            {"imdir", OrderDescending ? "descending" : "ascending"}
        };
        if (MatchingTitles is not null)
        {
            dict["imtitles"] = MediaWikiHelper.JoinValues(MatchingTitles);
        }
        return dict;
    }
}