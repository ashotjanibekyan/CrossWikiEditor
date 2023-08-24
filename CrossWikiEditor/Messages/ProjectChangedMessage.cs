namespace CrossWikiEditor.Messages;

public sealed class ProjectChangedMessage : BaseMessage
{
    public ProjectChangedMessage(ProjectEnum project)
    {
        Project = project;
    }

    public ProjectEnum Project { get; }
}