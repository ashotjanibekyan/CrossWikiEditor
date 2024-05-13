namespace CrossWikiEditor.Core;

/// <summary>
/// Message is fired when the page has been saved.
/// </summary>
/// <param name="wikiPageModel"></param>
public sealed class PageSavedMessage(WikiPageModel wikiPageModel, bool isSuccessful, Exception? exception = null)
{
    public WikiPageModel Page { get; } = wikiPageModel;
    public bool IsSuccessful { get; } = isSuccessful;
    public Exception? Exception { get; } = exception;
}