namespace CrossWikiEditor.Core.Messages;

public sealed class SaveOrSkipPageMessage
{
    public SaveOrSkipPageMessage(bool shouldSavePage)
    {
        ShouldSavePage = shouldSavePage;
    }

    public bool ShouldSavePage { get; }
}