namespace CrossWikiEditor.Core.Messages;

public sealed class LanguageCodeChangedMessage(string languageCode) : ValueChangedMessage<string>(languageCode)
{
}