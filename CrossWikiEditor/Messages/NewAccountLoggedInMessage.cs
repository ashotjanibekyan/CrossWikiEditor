using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.Messages;

public sealed class NewAccountLoggedInMessage(Profile profile) : ValueChangedMessage<Profile>(profile)
{
}