namespace CrossWikiEditor.Core;

public sealed class PageListProcessor
{
    private readonly IMessengerWrapper _messenger;
    private readonly ISettingsService _settingsService;
    private readonly UserSettings _userSettings;
    private readonly List<WikiPageModel> _pages;
    private bool _isAlive = true;
    private TaskCompletionSource<bool>? _shouldSaveTaskCompletionSource;

    public PageListProcessor(
        IMessengerWrapper messenger,
        ISettingsService settingsService,
        List<WikiPageModel> pages,
        NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        _messenger = messenger;
        _settingsService = settingsService;
        _userSettings = settingsService.GetCurrentSettings();
        _pages = pages;
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

        if (initialContent == newContent && _userSettings.SkipOptions.ShouldSkipIfNoChange)
        {
            _messenger.Send(new PageSkippedMessage(page, SkipReason.NoChanges));
            return;
        }

        if (!_userSettings.IsBotMode)
        {
            _shouldSaveTaskCompletionSource = new TaskCompletionSource<bool>();
            _messenger.Send(new PageUpdatingMessage(page, initialContent, newContent));
            if (!await _shouldSaveTaskCompletionSource.Task)
            {
                _messenger.Send(new PageSkippedMessage(page, SkipReason.Manual));
                return;
            }
        }

        await SavePage(page, newContent, GetSummary(replacements));
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
            foreach (NormalFindAndReplaceRule normalFindAndReplaceRule in _userSettings.NormalFindAndReplaceRules)
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
        catch (Exception)
        {
            _messenger.Send(new PageProcessedMessage(page, false));
            return (null, null, null);
        }
    }

    private async Task SavePage(WikiPageModel page, string newContent, string summary)
    {
        _messenger.Send(new PageSavingMessage(page));
        await page.SetContent(newContent);
        try
        {
            await page.UpdateContent(summary);
            _messenger.Send(new PageSavedMessage(page, true));
        }
        catch (Exception e)
        {
            _messenger.Send(new PageSavedMessage(page, false, e));
        }
    }

    private string GetSummary(List<Tuple<string, string>> replacements)
    {
        if (_userSettings.NormalFindAndReplaceRules.AddToSummary)
        {
            return string.Join(", ", replacements.Select(tp => $"{tp.Item1} \u2192 {tp.Item2}")) + ", օգտվելով CWE";
        }
        else
        {
            return "օգտվելով CWE";
        }
    }

    public void Stop()
    {
        _isAlive = false;
        _messenger.Unregister<SaveOrSkipPageMessage>(this);
    }
}