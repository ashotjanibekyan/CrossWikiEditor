using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CrossWikiEditor.Messages;

public sealed class LanguageCodeChangedMessage(string languageCode) : ValueChangedMessage<string>(languageCode)
{
}