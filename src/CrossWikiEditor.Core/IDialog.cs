namespace CrossWikiEditor.Core;

public interface IDialog
{
    void Close(object? dialogResult);
    object? DataContext { get; set; }
}

public interface IOwner { }