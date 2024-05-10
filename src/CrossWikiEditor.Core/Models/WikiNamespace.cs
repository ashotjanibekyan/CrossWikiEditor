namespace CrossWikiEditor.Core.Models;

public sealed class WikiNamespace(int id, string name, bool isChecked = false)
{
    public int Id { get; } = id;

    public string Name => name == "" ? "Main/Article" : name;

    public bool IsChecked { get; set; } = isChecked;
}