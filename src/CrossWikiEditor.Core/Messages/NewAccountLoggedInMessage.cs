namespace CrossWikiEditor.Core.Messages;

public sealed class NewAccountLoggedInMessage(Profile profile) : ValueChangedMessage<Profile>(profile)
{
}