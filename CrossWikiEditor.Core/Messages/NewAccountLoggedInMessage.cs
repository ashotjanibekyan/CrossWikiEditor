using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Messages;

public sealed class NewAccountLoggedInMessage(Profile profile) : ValueChangedMessage<Profile>(profile)
{
}