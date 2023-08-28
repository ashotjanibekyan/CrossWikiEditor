using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Settings;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ViewModels.ControlViewModels;

public partial class FindAndReplaceViewModel : ViewModelBase
{
    public FindAndReplaceViewModel()
    {
        var list = new List<NormalFindAndReplaceRule> {new()};
        NormalFindAndReplaceRules = list.ToObservableCollection();
        foreach (NormalFindAndReplaceRule model in NormalFindAndReplaceRules)
        {
            model.PropertyChanged += OnModelPropertyChanged;
        }
    }

    [RelayCommand]
    private void Save(IDialog dialog)
    {
        foreach (NormalFindAndReplaceRule rule in NormalFindAndReplaceRules)
        {
            rule.PropertyChanged -= OnModelPropertyChanged;
        }

        var rules = new NormalFindAndReplaceRules(NormalFindAndReplaceRules.Where(r => !r.IsEmpty))
        {
            IgnoreLinks = IgnoreLinks,
            IgnoreMore = IgnoreMore,
            AddToSummary = AddToSummary
        };
        
        dialog.Close(rules);        
    }

    [ObservableProperty] private ObservableCollection<NormalFindAndReplaceRule> _normalFindAndReplaceRules;
    [ObservableProperty] private bool _ignoreLinks;
    [ObservableProperty] private bool _ignoreMore;
    [ObservableProperty] private bool _addToSummary;

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (NormalFindAndReplaceRules.LastOrDefault() is not {IsEmpty: false})
        {
            return;
        }
        var newModel = new NormalFindAndReplaceRule();
        newModel.PropertyChanged += OnModelPropertyChanged;
        NormalFindAndReplaceRules.Add(newModel);
    }
}