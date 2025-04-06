using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Core.Settings;

namespace CrossWikiEditor.Core.Messages;

public sealed class CurrentSettingsUpdatedMessage : ValueChangedMessage<UserSettings>
{
    public CurrentSettingsUpdatedMessage(UserSettings newSettings) : base(newSettings)
    {
    }
}