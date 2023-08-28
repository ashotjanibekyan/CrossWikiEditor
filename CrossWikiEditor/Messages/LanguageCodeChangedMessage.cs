namespace CrossWikiEditor.Messages;

public sealed class LanguageCodeChangedMessage(string languageCode) : BaseMessage
{
    public string LanguageCode { get; } = languageCode;
}