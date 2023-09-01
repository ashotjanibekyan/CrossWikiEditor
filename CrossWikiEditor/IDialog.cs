using System.Threading.Tasks;
using Avalonia.Controls;

namespace CrossWikiEditor;

public interface IDialog
{
    void Close(object? dialogResult);
    object? DataContext { get; set; }
}

public interface IOwner { }

public static class DialogExtensions
{
    public static async Task<TResult> ShowDialog<TResult>(this IDialog dialog, IOwner owner)
    {
        var window = (Window) dialog;
        return await window.ShowDialog<TResult>((Window) owner);
    }

    public static async Task ShowDialog(this IDialog dialog, IOwner owner)
    {
        var window = (Window) dialog;
        await window.ShowDialog((Window) owner);
    }
}