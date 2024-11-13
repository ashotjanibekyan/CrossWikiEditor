using System.Reflection;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;
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
        _settingsService = settingsService;
        _generalOptions = _settingsService.GetCurrentSettings().GeneralOptions;
        NormalFindAndReplaceRules = [];
        PopulateProperties();
        PropertyChanged += OptionsViewModel_PropertyChanged;
    }

    [ObservableProperty] public partial bool AutoTag { get; set; }
    [ObservableProperty] public partial bool ApplyGeneralFixes { get; set; }
    [ObservableProperty] public partial bool UnicodifyWholePage { get; set; }
    [ObservableProperty] public partial bool FindAndReplace { get; set; }
    [ObservableProperty] public partial bool SkipIfNoReplacement { get; set; }
    [ObservableProperty] public partial bool SkipIfOnlyMinorReplacementMade { get; set; }
    [ObservableProperty] public partial bool RegexTypoFixing { get; set; }
    [ObservableProperty] public partial bool SkipIfNoTypoFixed { get; set; }
    [ObservableProperty] public partial NormalFindAndReplaceRules NormalFindAndReplaceRules { get; set; }

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

    private void OptionsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
        {
            return;
        }

        PropertyInfo property = typeof(OptionsViewModel).GetProperty(e.PropertyName!)!;
        PropertyInfo targetProperty = typeof(GeneralOptions).GetProperty(e.PropertyName)!;
        targetProperty.SetValue(_generalOptions, property.GetValue(this));
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