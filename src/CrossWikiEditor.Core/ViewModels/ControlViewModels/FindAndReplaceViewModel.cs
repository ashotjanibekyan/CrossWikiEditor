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

    [ObservableProperty] private ObservableCollection<NormalFindAndReplaceRule> _normalFindAndReplaceRules;
    [ObservableProperty] private bool _ignoreLinks;
    [ObservableProperty] private bool _ignoreMore;
    [ObservableProperty] private bool _addToSummary;

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (NormalFindAndReplaceRules.LastOrDefault() is not { IsEmpty: false })
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