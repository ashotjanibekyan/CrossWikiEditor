namespace CrossWikiEditor.Core.Messages;

public sealed class ProjectChangedMessage(ProjectEnum project) : ValueChangedMessage<ProjectEnum>(project)
{
}