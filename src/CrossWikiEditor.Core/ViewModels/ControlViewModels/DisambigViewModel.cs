using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Messages;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public sealed partial class DisambigViewModel : ViewModelBase
{
    private DisambigOptions _disambigOptions; 
    public DisambigViewModel(
        ISettingsService settingsService,
        IMessengerWrapper messenger)
    {
        messenger.Register<CurrentSettingsUpdatedMessage>(this, (r, m) =>
        {
            _disambigOptions = settingsService.GetCurrentSettings().DisambigOptions;
            PopulateProperties();
        });
        _disambigOptions = settingsService.GetCurrentSettings().DisambigOptions;
        PopulateProperties();
    }
    [ObservableProperty] public partial bool EnableDisambiguation { get; set; }
    partial void OnEnableDisambiguationChanged(bool value) => _disambigOptions.EnableDisambiguation = value;
    
    [ObservableProperty] public partial string LinkToDisambiguate { get; set; } = string.Empty;
    partial void OnLinkToDisambiguateChanged(string value) => _disambigOptions.LinkToDisambiguate = value;
    
    [ObservableProperty] public partial string SkipPageNoDisambiguationsMade { get; set; } = string.Empty;
    partial void OnSkipPageNoDisambiguationsMadeChanged(string value) => _disambigOptions.SkipPageNoDisambiguationsMade = value;
    
    [ObservableProperty] public partial int ContextCharacterCount { get; set; }
    partial void OnContextCharacterCountChanged(int value) => _disambigOptions.ContextCharacterCount = value;
    
    [RelayCommand]
    private async Task Load()
    {
        await Task.Delay(500);
    }

    private void PopulateProperties()
    {
        EnableDisambiguation = _disambigOptions.EnableDisambiguation;
        LinkToDisambiguate = _disambigOptions.LinkToDisambiguate;
        SkipPageNoDisambiguationsMade = _disambigOptions.SkipPageNoDisambiguationsMade;
        ContextCharacterCount = _disambigOptions.ContextCharacterCount;
    }
}