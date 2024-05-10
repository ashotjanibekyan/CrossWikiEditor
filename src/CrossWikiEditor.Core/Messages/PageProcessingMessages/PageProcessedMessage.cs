namespace CrossWikiEditor.Core;

/// <summary>
/// Message is fired when the local processing is finished and the page is ready to be saved.
/// </summary>
/// <param name="wikiPageModel"></param>
public sealed class PageProcessedMessage(WikiPageModel wikiPageModel, bool isSuccessful)
{
    public WikiPageModel Page { get; } = wikiPageModel;
    public bool IsSuccessful { get; } = isSuccessful;
}