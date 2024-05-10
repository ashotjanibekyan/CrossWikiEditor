namespace CrossWikiEditor.Core.Messages;

public sealed class SaveOrSkipPageMessage(bool shouldSavePage)
{
    public bool ShouldSavePage { get; } = shouldSavePage;
}