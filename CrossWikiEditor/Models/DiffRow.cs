namespace CrossWikiEditor.Models;

public class DiffRow
{
    public DiffRowNode First { get; set; } = new();
    public DiffRowNode Second { get; set; } = new();
    public DiffRowNode Third { get; set; } = new();
    public DiffRowNode Forth { get; set; } = new();

    public void AddNode(DiffRowNode node, int index)
    {
        switch (index)
        {
            case 1:
                First = node;
                break;
            case 2:
                Second = node;
                break;
            case 3:
                Third = node;
                break;
            case 4:
                Forth = node;
                break;
        }
    }
}