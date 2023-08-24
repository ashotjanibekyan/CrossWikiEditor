namespace CrossWikiEditor.Messages;

public sealed class LanguageCodeChangedMessage
{
    public LanguageCodeChangedMessage(string languageCode)
    {
        LanguageCode = languageCode;
    }
    public string LanguageCode { get; }
}