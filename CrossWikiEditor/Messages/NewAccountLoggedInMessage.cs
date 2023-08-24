using CrossWikiEditor.Models;

namespace CrossWikiEditor.Messages;

public sealed class NewAccountLoggedInMessage : BaseMessage
{
    public NewAccountLoggedInMessage(Profile profile)
    {
        Profile = profile;
    }
    public Profile Profile { get; }
}