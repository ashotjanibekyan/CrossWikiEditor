namespace CrossWikiEditor.Core;

public interface IDialog
{
    object? DataContext { get; set; }
    void Close(object? dialogResult);
}

public interface IOwner;