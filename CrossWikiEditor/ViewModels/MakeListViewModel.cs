using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class MakeListViewModel : ViewModelBase
{
    public MakeListViewModel()
    {
        AddNewPageCommand = ReactiveCommand.Create(AddNewPage);
        RemovePageCommand = ReactiveCommand.Create(RemovePage);
    }

    private void RemovePage()
    {
        if (!string.IsNullOrWhiteSpace(SelectedPage) && Pages.Contains(SelectedPage))
        {
            Pages.Remove(SelectedPage);
        }

        SelectedPage = string.Empty;
    }

    private void AddNewPage()
    {
        if (!string.IsNullOrWhiteSpace(NewPageTitle) && !Pages.Contains(NewPageTitle.Trim()))
        {
            Pages.Add(NewPageTitle.Trim());
        }

        NewPageTitle = string.Empty;
    }

    public IEnumerable<string> PageGenerators { get; }
    [Reactive] public ObservableCollection<string> Pages { get; set; } = new();
    [Reactive] public string NewPageTitle { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> AddNewPageCommand { get; }
    public string SelectedPage { get; set; }
    public ReactiveCommand<Unit, Unit> RemovePageCommand { get; set; }
}