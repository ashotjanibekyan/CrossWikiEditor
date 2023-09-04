using System.Text.RegularExpressions;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Core;

public class PageListProcessor
{
    private readonly IMessengerWrapper _messenger;
    private readonly List<WikiPageModel> _pages;
    private readonly NormalFindAndReplaceRules _normalFindAndReplaceRules;
    private bool _isAlive = true;

    public PageListProcessor(IMessengerWrapper messenger, List<WikiPageModel> pages, NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        _messenger = messenger;
        _pages = pages;
        _normalFindAndReplaceRules = normalFindAndReplaceRules;
        messenger.Register<StopBotMessage>(this, (recipient, message) =>
        {
            _isAlive = false;
        });
    }

    public async Task Start()
    {
        foreach (WikiPageModel page in _pages.TakeWhile(_ => _isAlive))
        {
            await ProcessPage(page);
        }
    }

    private async Task ProcessPage(WikiPageModel page)
    {
        if (string.IsNullOrEmpty(await page.GetContent()))
        {
            await page.RefreshAsync(PageQueryOptions.FetchContent);
        }
        string initialContent = await page.GetContent();
        string newContent = await page.GetContent();
        foreach (NormalFindAndReplaceRule normalFindAndReplaceRule in _normalFindAndReplaceRules)
        {
            if (!normalFindAndReplaceRule.Regex)
            {
                newContent = newContent.Replace((string) normalFindAndReplaceRule.Find, normalFindAndReplaceRule.ReplaceWith);
            }
            else
            {
                var regex = new Regex(normalFindAndReplaceRule.Find);
                regex.Replace(newContent, (string) normalFindAndReplaceRule.ReplaceWith);
            }
        }

        await page.SetContent(newContent);
        try
        {
            await Task.Delay(1000);
            //await page.WikiPage.UpdateContentAsync("via CWB");
            _messenger.Send(new PageUpdatedMessage(page, initialContent, newContent));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }
}