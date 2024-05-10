namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel(
    NormalFindAndReplaceRules normalFindAndReplaceRules,
    IDialogService dialogService) : ViewModelBase
{
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; private set; } = normalFindAndReplaceRules;

    [RelayCommand]
    private async Task OpenNormalFindAndReplaceDialog()
    {
        NormalFindAndReplaceRules? result =
            await dialogService.ShowDialog<NormalFindAndReplaceRules>(new FindAndReplaceViewModel(NormalFindAndReplaceRules));
        if (result is not null)
        {
            NormalFindAndReplaceRules = result;
        }
    }
}