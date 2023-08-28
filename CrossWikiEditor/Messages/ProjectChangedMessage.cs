namespace CrossWikiEditor.Messages;

public sealed class ProjectChangedMessage(ProjectEnum project) : BaseMessage
{
    public ProjectEnum Project { get; } = project;
}