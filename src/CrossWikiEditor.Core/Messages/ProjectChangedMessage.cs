using CommunityToolkit.Mvvm.Messaging.Messages;
using CrossWikiEditor.Core.Settings;

namespace CrossWikiEditor.Core.Messages;

public sealed class ProjectChangedMessage : ValueChangedMessage<ProjectEnum>
{
    public ProjectChangedMessage(ProjectEnum project) : base(project)
    {
    }
}