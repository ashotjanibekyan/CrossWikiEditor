namespace CrossWikiEditor.Core;

public sealed class PageListProcessor
{
    private readonly IMessengerWrapper _messenger;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly NormalFindAndReplaceRules _normalFindAndReplaceRules;
    private readonly List<WikiPageModel> _pages;
    private bool _isAlive = true;
    private TaskCompletionSource<bool>? _shouldSaveTaskCompletionSource;

    public PageListProcessor(
        IMessengerWrapper messenger,
        IUserPreferencesService userPreferencesService,
        List<WikiPageModel> pages,
        NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        _messenger = messenger;
        _userPreferencesService = userPreferencesService;
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
                await TreatPage(page);
            }
            catch (Exception)
            {
                _messenger.Unregister<SaveOrSkipPageMessage>(this);
            }
        }
    }

    private async Task TreatPage(WikiPageModel page)
    {
        var (initialContent, newContent, replacements) = await ProcessPage(page);
        if (initialContent is null || newContent is null || replacements is null)
        {
            _messenger.Send(new PageSkippedMessage(page, SkipReason.ErrorProcessing));
            return;
        }

        if (initialContent == newContent)
        {
            _messenger.Send(new PageSkippedMessage(page, SkipReason.NoChanges));
            _messenger.Send(new PageUpdatingMessage(page, initialContent, newContent));
            return;
        }
        


        if (!_userPreferencesService.GetCurrentSettings().IsBotMode)
        {
            _shouldSaveTaskCompletionSource = new TaskCompletionSource<bool>();
            _messenger.Send(new PageUpdatingMessage(page, initialContent, newContent));
            if (!await _shouldSaveTaskCompletionSource.Task)
            {
                _messenger.Send(new PageSkippedMessage(page, SkipReason.Manual));
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

    private async Task<(string? initialTxt, string? newTxt, List<Tuple<string, string>>? repls)> ProcessPage(WikiPageModel page)
    {
        try
        {
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
            _messenger.Send(new PageProcessedMessage(page, true));
            return (initialContent, newContent, replacements);
        }
        catch(Exception)
        {
            _messenger.Send(new PageProcessedMessage(page, false));
            return (null, null, null);
        }
    }

    public void Stop()
    {
        _isAlive = false;
        _messenger.Unregister<SaveOrSkipPageMessage>(this);
    }
}