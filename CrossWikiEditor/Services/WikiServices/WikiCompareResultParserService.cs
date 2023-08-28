using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                diffNodeRow.HtmlClasses = td.GetClasses().ToList();
                foreach (var child in td.ChildNodes)
                {
                    if (child.Name == "div")
                    {
                        foreach (HtmlNode? dicChild in child.ChildNodes)
                        {
                            diffNodeRow.ContentNodes.Add(new ContentNode()
                            {
                                Value = HttpUtility.HtmlDecode(dicChild.InnerHtml),
                                Classes = dicChild.GetClasses().ToList(),
                                Name = dicChild.Name
                            });
                        }
                    }
                    else
                    {
                        diffNodeRow.ContentNodes.Add(new ContentNode()
                        {
                            Value = child.InnerText,
                            Classes = child.GetClasses().ToList(),
                            Name = child.Name
                        });
                    }
                }
                foreach (HtmlAttribute attribute in td.Attributes)
                {
                    switch (attribute.Name)
                    {
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