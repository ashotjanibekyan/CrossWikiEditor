namespace CrossWikiEditor.Core;

public sealed class PageListProcessor
{
    private readonly IMessengerWrapper _messenger;
    private readonly List<WikiPageModel> _pages;
    private readonly NormalFindAndReplaceRules _normalFindAndReplaceRules;
    private bool _isAlive = true;
    private TaskCompletionSource<bool>? _shouldSaveTaskCompletionSource = null;

    public PageListProcessor(
        IMessengerWrapper messenger, 
        List<WikiPageModel> pages, 
        NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        _messenger = messenger;
        _pages = pages;
        _normalFindAndReplaceRules = normalFindAndReplaceRules;
        messenger.Register<StopBotMessage>(this, (recipient, message) =>
        {
            _isAlive = false;
        });
        messenger.Register<SaveOrSkipPageMessage>(this, (recipient, message) =>
        {
            _shouldSaveTaskCompletionSource?.SetResult(message.ShouldSavePage);
        });
    }

    public async Task Start()
    {
        try
        {
            foreach (WikiPageModel page in _pages.TakeWhile(_ => _isAlive))
            {
                await ProcessPage(page);
            }
        }
        catch (Exception)
        {
            _messenger.Unregister<StopBotMessage>(this);
            _messenger.Unregister<SaveOrSkipPageMessage>(this);
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
                newContent = regex.Replace(newContent, (string) normalFindAndReplaceRule.ReplaceWith);
            }
        }

        if (initialContent == newContent)
        {
            _messenger.Send(new PageSkippedMessage(page, "No changes"));
            return;
        }

        if (true) // TODO: check if bot mode is not enabled
        {
            _shouldSaveTaskCompletionSource = new TaskCompletionSource<bool>();
            _messenger.Send(new PageUpdatingMessage(page, initialContent, newContent));
            if (!await _shouldSaveTaskCompletionSource.Task)
            {
                _messenger.Send(new PageSkippedMessage(page, "Manual skip"));
                return;
            }
        }
        
        await page.SetContent(newContent);
        try
        {
            await Task.Delay(1000);
            await page.UpdateContent("via CWB");
            _messenger.Send(new PageUpdatedMessage(page));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}