using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CrossWikiEditor.Core.Messages;

public sealed class LanguageCodeChangedMessage : ValueChangedMessage<string>
{
    public LanguageCodeChangedMessage(string languageCode) : base(languageCode)
    {
    }
}