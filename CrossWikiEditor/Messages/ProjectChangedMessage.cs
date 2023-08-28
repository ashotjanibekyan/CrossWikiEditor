using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CrossWikiEditor.Messages;

public sealed class ProjectChangedMessage(ProjectEnum project) : ValueChangedMessage<ProjectEnum>(project)
{
}