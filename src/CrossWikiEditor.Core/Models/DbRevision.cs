namespace CrossWikiEditor.Core.Models;

public sealed class DbRevision
{
    public int Id { get; set; } 
    public DateTime Timestamp { get; set; } 
    public DbContributor? Contributor { get; set; } 
    public string? Comment { get; set; } 
    public string? Model { get; set; } 
    public string? Format { get; set; } 
    public string? Text { get; set; } 
    public long? TextSize { get; set; } 
    public string? Sha1 { get; set; } 
    public int Parentid { get; set; } 
}