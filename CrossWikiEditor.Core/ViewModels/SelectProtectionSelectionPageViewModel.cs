using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectProtectionSelectionPageViewModel : ViewModelBase
{
    [ObservableProperty] private int _protectionType;
    [ObservableProperty] private int _protectionLevel;

    [RelayCommand]
    private void Ok(IDialog dialog)
    {
        dialog.Close((ProtectionType switch
        {
            0 => "edit",
            1 => "move",
            2 => "edit|move",
            _ => ""
        }, ProtectionLevel switch
        {
            0 => "autoconfirmed",
            1 => "sysop",
            2 => "autoconfirmed|sysop",
            _ => ""
        }));
    }
}