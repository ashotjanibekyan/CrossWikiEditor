using CrossWikiEditor.Models;

namespace CrossWikiEditor.Messages;

public class NewAccountLoggedInMessage
{
    public NewAccountLoggedInMessage(Profile profile)
    {
        Profile = profile;
    }
    public Profile Profile { get; }
}