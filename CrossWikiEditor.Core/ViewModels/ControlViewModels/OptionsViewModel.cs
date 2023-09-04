using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Settings;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel(IDialogService dialogService) : ViewModelBase
{
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; private set; } = new();

    [RelayCommand]
    private async Task OpenNormalFindAndReplaceDialog()
    {
        NormalFindAndReplaceRules? result = await dialogService.ShowDialog<NormalFindAndReplaceRules>(new FindAndReplaceViewModel());
        if (result is not null)
        {
            NormalFindAndReplaceRules = result;
        }
    }
}