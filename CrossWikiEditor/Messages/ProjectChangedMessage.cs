namespace CrossWikiEditor.Messages;

public class ProjectChangedMessage
{
    public ProjectChangedMessage(ProjectEnum project)
    {
        Project = project;
    }

    public ProjectEnum Project { get; }
}