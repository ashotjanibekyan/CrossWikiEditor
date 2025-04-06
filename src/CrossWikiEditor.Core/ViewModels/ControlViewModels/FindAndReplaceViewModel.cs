using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ViewModels.ControlViewModels;

public partial class FindAndReplaceViewModel : ViewModelBase
{
    public FindAndReplaceViewModel(NormalFindAndReplaceRules normalFindAndReplaceRules)
    {
        NormalFindAndReplaceRules = normalFindAndReplaceRules.ToObservableCollection();
        foreach (NormalFindAndReplaceRule model in NormalFindAndReplaceRules)
        {
            model.PropertyChanged += OnModelPropertyChanged;
        }

        AddNewRow();
        IgnoreLinks = normalFindAndReplaceRules.IgnoreLinks;
        IgnoreMore = normalFindAndReplaceRules.IgnoreMore;
        AddToSummary = normalFindAndReplaceRules.AddToSummary;
    }

    [ObservableProperty] public partial ObservableCollection<NormalFindAndReplaceRule> NormalFindAndReplaceRules { get; set; }

    [ObservableProperty] public partial bool IgnoreLinks { get; set; }

    [ObservableProperty] public partial bool IgnoreMore { get; set; }

    [ObservableProperty] public partial bool AddToSummary { get; set; }

    [RelayCommand]
    private void Clean()
    {
        NormalFindAndReplaceRules.Clear();
        AddNewRow();
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

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (NormalFindAndReplaceRules.LastOrDefault() is not {IsEmpty: false})
        {
            return;
        }

        AddNewRow();
    }

    private void AddNewRow()
    {
        var newModel = new NormalFindAndReplaceRule();
        newModel.PropertyChanged += OnModelPropertyChanged;
        NormalFindAndReplaceRules.Add(newModel);
    }
}