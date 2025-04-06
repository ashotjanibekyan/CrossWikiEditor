using System;
using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core;

/// <summary>
///     Message is fired when the page has been saved.
/// </summary>
public sealed class PageSavedMessage
{
    /// <summary>
    ///     Message is fired when the page has been saved.
    /// </summary>
    /// <param name="wikiPageModel"></param>
    public PageSavedMessage(WikiPageModel wikiPageModel, bool isSuccessful, Exception? exception = null)
    {
        Page = wikiPageModel;
        IsSuccessful = isSuccessful;
        Exception = exception;
    }

    public WikiPageModel Page { get; }
    public bool IsSuccessful { get; }
    public Exception? Exception { get; }
}