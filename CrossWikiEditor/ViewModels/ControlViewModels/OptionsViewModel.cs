using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Services;
using CrossWikiEditor.Settings;

namespace CrossWikiEditor.ViewModels.ControlViewModels;

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