using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Settings;

namespace CrossWikiEditor.Messages;

public sealed class ProjectChangedMessage(ProjectEnum project) : ValueChangedMessage<ProjectEnum>(project)
{
}