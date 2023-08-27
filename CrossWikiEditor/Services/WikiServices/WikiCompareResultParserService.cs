using System;
using System.Collections.Generic;
using CrossWikiEditor.Models;
using HtmlAgilityPack;

namespace CrossWikiEditor.Services.WikiServices;

public class WikiCompareResultParserService
{

    public List<DiffRow> ParseCompareHtml(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        HtmlNodeCollection? trs = htmlDoc.DocumentNode.SelectNodes("tr");

        var data = new List<DiffRow>();

        foreach (HtmlNode tr in trs)
        {
            HtmlNodeCollection? tds = tr.SelectNodes("td");
            var col = new DiffRow();
            int index = 1;
            foreach (HtmlNode td in tds)
            {
                if (td.OuterHtml.Contains("data-marker="))
                {
                    Console.WriteLine();
                }
                var diffNodeRow = new DiffRowNode();
                diffNodeRow.Content = td.InnerHtml;
                foreach (HtmlAttribute attribute in td.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case "class":
                            diffNodeRow.HtmlClasses.Add(attribute.Value);
                            break;
                        case "colspan":
                            diffNodeRow.ColSpan = Convert.ToInt32(attribute.Value);
                            break;
                        case "data-marker":
                            diffNodeRow.Marker = attribute.Value;
                            break;
                    }
                }
                col.AddNode(diffNodeRow, index);
                index+=diffNodeRow.ColSpan;
                
            }
            data.Add(col);
        }

        return data;
    }
}