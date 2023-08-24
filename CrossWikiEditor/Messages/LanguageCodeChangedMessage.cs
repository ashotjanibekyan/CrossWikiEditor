namespace CrossWikiEditor.Messages;

public class LanguageCodeChangedMessage
{
    public LanguageCodeChangedMessage(string languageCode)
    {
        LanguageCode = languageCode;
    }
    public string LanguageCode { get; }
}