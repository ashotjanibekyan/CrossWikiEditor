namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel(IDialogService dialogService) : ViewModelBase
{
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; private set; } = new();

    [RelayCommand]
    private async Task OpenNormalFindAndReplaceDialog()
    {
        NormalFindAndReplaceRules? result = await dialogService.ShowDialog<NormalFindAndReplaceRules>(new FindAndReplaceViewModel(NormalFindAndReplaceRules));
        if (result is not null)
        {
            NormalFindAndReplaceRules = result;
        }
    }
}