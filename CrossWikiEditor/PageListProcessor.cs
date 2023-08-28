using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CrossWikiEditor.Messages;
using CrossWikiEditor.Models;
using CrossWikiEditor.Settings;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor;

public class PageListProcessor
{
    private readonly IMessenger _messenger;
    private readonly List<WikiPageModel> _pages;
    private readonly NormalFindAndReplaceRules _normalFindAndReplaceRules;
    private bool _isAlive = true;

    public PageListProcessor(IMessenger messenger, List<WikiPageModel> pages, NormalFindAndReplaceRules normalFindAndReplaceRules)
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
        if (page.WikiPage is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(page.WikiPage.Content))
        {
            await page.WikiPage.RefreshAsync(PageQueryOptions.FetchContent);
        }
        string initialContent = page.WikiPage.Content;
        string newContent = page.WikiPage.Content;
        foreach (NormalFindAndReplaceRule normalFindAndReplaceRule in _normalFindAndReplaceRules)
        {
            if (!normalFindAndReplaceRule.Regex)
            {
                newContent = newContent.Replace(normalFindAndReplaceRule.Find, normalFindAndReplaceRule.ReplaceWith);
            }
            else
            {
                var regex = new Regex(normalFindAndReplaceRule.Find);
                regex.Replace(newContent, normalFindAndReplaceRule.ReplaceWith);
            }
        }

        page.WikiPage.Content = newContent;
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