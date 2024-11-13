namespace CrossWikiEditor.Core.ViewModels;

public sealed partial class SelectProtectionSelectionPageViewModel : ViewModelBase
{
    [ObservableProperty] public partial int ProtectionType { get; set; }

    [ObservableProperty] public partial int ProtectionLevel { get; set; }

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