using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private GeneralOptions _generalOptions;

    public OptionsViewModel(
        ISettingsService settingsService,
        IDialogService dialogService,
        IMessengerWrapper messenger)
    {
        messenger.Register<CurrentSettingsUpdatedMessage>(this, (r, m) =>
        {
            _generalOptions = settingsService.GetCurrentSettings().GeneralOptions;
            PopulateProperties();
        });
        _dialogService = dialogService;
        _generalOptions = settingsService.GetCurrentSettings().GeneralOptions;
        NormalFindAndReplaceRules = [];
        PopulateProperties();
    }

    
    [ObservableProperty] public partial bool AutoTag { get; set; }
    partial void OnAutoTagChanged(bool value) => _generalOptions.AutoTag = value;
    
    [ObservableProperty] public partial bool ApplyGeneralFixes { get; set; }
    partial void OnApplyGeneralFixesChanged(bool value) => _generalOptions.ApplyGeneralFixes = value;
    
    [ObservableProperty] public partial bool UnicodifyWholePage { get; set; }
    partial void OnUnicodifyWholePageChanged(bool value) => _generalOptions.UnicodifyWholePage = value;
    
    [ObservableProperty] public partial bool FindAndReplace { get; set; }
    partial void OnFindAndReplaceChanged(bool value) => _generalOptions.FindAndReplace = value;
    
    [ObservableProperty] public partial bool SkipIfNoReplacement { get; set; }
    partial void OnSkipIfNoReplacementChanged(bool value) => _generalOptions.SkipIfNoReplacement = value;
    
    [ObservableProperty] public partial bool SkipIfOnlyMinorReplacementMade { get; set; }
    partial void OnSkipIfOnlyMinorReplacementMadeChanged(bool value) => _generalOptions.SkipIfOnlyMinorReplacementMade = value;
    
    [ObservableProperty] public partial bool RegexTypoFixing { get; set; }
    partial void OnRegexTypoFixingChanged(bool value) => _generalOptions.RegexTypoFixing = value;
    
    [ObservableProperty] 
    public partial bool SkipIfNoTypoFixed { get; set; }
    partial void OnSkipIfNoTypoFixedChanged(bool value) => _generalOptions.SkipIfNoTypoFixed = value;
    
    [ObservableProperty] public partial NormalFindAndReplaceRules NormalFindAndReplaceRules { get; set; }
    partial void OnNormalFindAndReplaceRulesChanged(NormalFindAndReplaceRules value) => _generalOptions.NormalFindAndReplaceRules = value;

    [RelayCommand]
    private async Task OpenNormalFindAndReplaceDialog()
    {
        NormalFindAndReplaceRules? result =
            await _dialogService.ShowDialog<NormalFindAndReplaceRules>(new FindAndReplaceViewModel(NormalFindAndReplaceRules));
        if (result is not null)
        {
            NormalFindAndReplaceRules = result;
        }
    }

    private void PopulateProperties()
    {
        NormalFindAndReplaceRules = _generalOptions.NormalFindAndReplaceRules;
        AutoTag = _generalOptions.AutoTag;
        ApplyGeneralFixes = _generalOptions.ApplyGeneralFixes;
        UnicodifyWholePage = _generalOptions.UnicodifyWholePage;
        FindAndReplace = _generalOptions.FindAndReplace;
        SkipIfNoReplacement = _generalOptions.SkipIfNoReplacement;
        SkipIfOnlyMinorReplacementMade = _generalOptions.SkipIfOnlyMinorReplacementMade;
        RegexTypoFixing = _generalOptions.RegexTypoFixing;
        SkipIfNoTypoFixed = _generalOptions.SkipIfNoTypoFixed;
    }
}