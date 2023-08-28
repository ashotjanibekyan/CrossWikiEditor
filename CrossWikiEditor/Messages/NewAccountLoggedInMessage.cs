using CrossWikiEditor.Models;

namespace CrossWikiEditor.Messages;

public sealed class NewAccountLoggedInMessage(Profile profile) : BaseMessage
{
    public Profile Profile { get; } = profile;
}