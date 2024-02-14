namespace CrossWikiEditor.Core;

public sealed class PageListProcessor
{
    private readonly IMessengerWrapper _messenger;
    private readonly NormalFindAndReplaceRules _normalFindAndReplaceRules;
    private readonly List<WikiPageModel> _pages;
    private bool _isAlive = true;
    private TaskCompletionSource<bool>? _shouldSaveTaskCompletionSource;

    public PageListProcessor(
        IMessengerWrapper messenger,
        List<WikiPageModel> pages,
        NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        _messenger = messenger;
        _pages = pages;
        _normalFindAndReplaceRules = normalFindAndReplaceRules;
        messenger.Register<SaveOrSkipPageMessage>(this,
            (recipient, message) => _shouldSaveTaskCompletionSource?.TrySetResult(message.ShouldSavePage));
    }

    public async Task Start()
    {
        foreach (WikiPageModel page in _pages.TakeWhile(_ => _isAlive))
        {
            try
            {
                await ProcessPage(page);
            }
            catch (Exception)
            {
                _messenger.Unregister<SaveOrSkipPageMessage>(this);
            }
        }
    }

    private async Task ProcessPage(WikiPageModel page)
    {
        Console.WriteLine($"Prccessing {page.Title}");
        if (string.IsNullOrEmpty(await page.GetContent()))
        {
            await page.RefreshAsync(PageQueryOptions.FetchContent);
        }

        string initialContent = await page.GetContent();
        string newContent = await page.GetContent();
        var replacements = new List<Tuple<string, string>>();
        foreach (NormalFindAndReplaceRule normalFindAndReplaceRule in _normalFindAndReplaceRules)
        {
            if (!normalFindAndReplaceRule.Regex)
            {
                newContent = newContent.Replace(normalFindAndReplaceRule.Find, normalFindAndReplaceRule.ReplaceWith);
            }
            else
            {
                var regex = new Regex(normalFindAndReplaceRule.Find);
                newContent = regex.Replace(newContent, match =>
                {
                    string replacedValue = normalFindAndReplaceRule.ReplaceWith;
                    replacedValue = Regex.Replace(replacedValue, @"\$([1-9])", groupReference => match.Groups[int.Parse(groupReference.Groups[1].Value)].Value);
                    replacements.Add(Tuple.Create(match.Value, replacedValue));
                    return replacedValue;
                });
            }
        }

        if (initialContent == newContent)
        {
            _messenger.Send(new PageSkippedMessage(page, "No changes"));
            _messenger.Send(new PageUpdatingMessage(page, initialContent, newContent));
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
            if (_normalFindAndReplaceRules.AddToSummary)
            {
                await page.UpdateContent(string.Join(", ", replacements.Select(tp => $"{tp.Item1} \u2192 {tp.Item2}")) + ", օգտվելով CWE");
            }
            else
            {
                await page.UpdateContent("օգտվելով CWE");
            }
            _messenger.Send(new PageUpdatedMessage(page));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Stop()
    {
        _isAlive = false;
        _messenger.Unregister<SaveOrSkipPageMessage>(this);
    }
}