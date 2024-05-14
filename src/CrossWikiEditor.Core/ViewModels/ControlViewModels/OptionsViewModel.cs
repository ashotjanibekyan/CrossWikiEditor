using System.Reflection;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class OptionsViewModel : ViewModelBase
{
    private GeneralOptions? _generalOptions;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;

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
        PopulateProperties();
        PropertyChanged += OptionsViewModel_PropertyChanged;
    }

    [ObservableProperty] private bool _autoTag;
    [ObservableProperty] private bool _applyGeneralFixes;
    [ObservableProperty] private bool _unicodifyWholePage;
    [ObservableProperty] private bool _findAndReplace;
    [ObservableProperty] private bool _skipIfNoReplacement;
    [ObservableProperty] private bool _skipIfOnlyMinorReplacementMade;
    [ObservableProperty] private bool _regexTypoFixing;
    [ObservableProperty] private bool _skipIfNoTypoFixed;
    [ObservableProperty] private NormalFindAndReplaceRules _normalFindAndReplaceRules = [];


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
        _generalOptions = _settingsService.GetCurrentSettings().GeneralOptions;
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