namespace CrossWikiEditor.Messages;

public sealed class LanguageCodeChangedMessage : BaseMessage
{
    public LanguageCodeChangedMessage(string languageCode)
    {
        LanguageCode = languageCode;
    }

    public string LanguageCode { get; }
}