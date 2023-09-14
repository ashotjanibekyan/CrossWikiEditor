using CrossWikiEditor.Core;
using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;
using CrossWikiEditor.Views;
using CrossWikiEditor.Views.ControlViews;
using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor.AutofacModules;

public static class DialogsModule
{
    public static void Register(IServiceCollection services)
    {
        services.AddKeyedTransient<IDialog, AddNewProfileView>(nameof(AddOrEditProfileViewModel));
        services.AddKeyedTransient<IDialog, AlertView>(nameof(AlertViewModel));
        services.AddKeyedTransient<IDialog, DatabaseScannerView>(nameof(DatabaseScannerViewModel));
        services.AddKeyedTransient<IDialog, FilterView>(nameof(FilterViewModel));
        services.AddKeyedTransient<IDialog, FindAndReplaceView>(nameof(FindAndReplaceViewModel));
        services.AddKeyedTransient<IDialog, PreferencesView>(nameof(PreferencesViewModel));
        services.AddKeyedTransient<IDialog, ProfilesView>(nameof(ProfilesViewModel));
        services.AddKeyedTransient<IDialog, PromptView>(nameof(PromptViewModel));
        services.AddKeyedTransient<IDialog, SelectNamespacesAndRedirectFilterView>(nameof(SelectNamespacesAndRedirectFilterViewModel));
        services.AddKeyedTransient<IDialog, SelectNamespacesView>(nameof(SelectNamespacesViewModel));
        services.AddKeyedTransient<IDialog, SelectProtectionSelectionPageView>(nameof(SelectProtectionSelectionPageViewModel));
    }
}