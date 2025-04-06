namespace CrossWikiEditor.Core.Models;

public sealed class WikiNamespace
{
    public WikiNamespace(int id, string name, bool isChecked = false)
    {
        Name = name;
        Id = id;
        IsChecked = isChecked;
    }

    public int Id { get; }

    public string Name => field == "" ? "Main/Article" : field;

    public bool IsChecked { get; set; }
}