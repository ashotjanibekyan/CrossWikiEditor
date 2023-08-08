namespace CrossWikiEditor.Models;

public class Profile
{
    public int Id { get; set; }
    public string Username { get; set; }
    public bool IsPasswordSaved { get; set; }
    public string? Password { get; set; }
    public string DefaultSettingsPath { get; set; }
    public string Notes { get; set; }
}