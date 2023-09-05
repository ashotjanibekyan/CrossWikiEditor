using Autofac;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.ViewModels;
using CrossWikiEditor.Core.ViewModels.ControlViewModels;
using CrossWikiEditor.Views;
using CrossWikiEditor.Views.ControlViews;

namespace CrossWikiEditor.AutofacModules;

public class DialogsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AlertView>().Named<IDialog>(nameof(AlertViewModel));
        builder.RegisterType<PromptView>().Named<IDialog>(nameof(PromptViewModel));
        builder.RegisterType<FilterView>().Named<IDialog>(nameof(FilterViewModel));
        builder.RegisterType<ProfilesView>().Named<IDialog>(nameof(ProfilesViewModel));
        builder.RegisterType<PreferencesView>().Named<IDialog>(nameof(PreferencesViewModel));
        builder.RegisterType<AddNewProfileView>().Named<IDialog>(nameof(AddOrEditProfileViewModel));
        builder.RegisterType<SelectNamespacesView>().Named<IDialog>(nameof(SelectNamespacesViewModel));
        builder.RegisterType<SelectNamespacesAndRedirectFilterView>().Named<IDialog>(nameof(SelectNamespacesAndRedirectFilterViewModel));
        builder.RegisterType<FindAndReplaceView>().Named<IDialog>(nameof(FindAndReplaceViewModel));
    }
}