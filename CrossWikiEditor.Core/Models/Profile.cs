namespace CrossWikiEditor.Core.Models;

public sealed class Profile
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsPasswordSaved { get; set; }
    public string Password { get; set; } = string.Empty;
    public string DefaultSettingsPath { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}