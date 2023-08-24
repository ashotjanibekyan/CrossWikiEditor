namespace CrossWikiEditor.Messages;

public sealed class ProjectChangedMessage
{
    public ProjectChangedMessage(ProjectEnum project)
    {
        Project = project;
    }

    public ProjectEnum Project { get; }
}