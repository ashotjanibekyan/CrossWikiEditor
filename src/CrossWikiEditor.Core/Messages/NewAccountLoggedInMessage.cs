using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public sealed class NewAccountLoggedInMessage : ValueChangedMessage<Profile>
{
    public NewAccountLoggedInMessage(Profile profile) : base(profile)
    {
    }
}