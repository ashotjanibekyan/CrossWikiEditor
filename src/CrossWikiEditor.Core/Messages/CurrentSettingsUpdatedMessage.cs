namespace CrossWikiEditor.Core.Messages;

public sealed class CurrentSettingsUpdatedMessage(UserSettings newSettings) : ValueChangedMessage<UserSettings>(newSettings)
{
}