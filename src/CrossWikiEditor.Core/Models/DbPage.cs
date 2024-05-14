namespace CrossWikiEditor.Core.Models;

public sealed class DbPage
{
    public required string Title { get; set; }
    public required int Ns { get; set; }
    public required int Id { get; set; }
    public List<DbRevision> Revision { get; set; } = [];
}