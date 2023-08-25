using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Services;
using CrossWikiEditor.Utils;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class MakeListViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MakeListViewModel(IDialogService dialogService, IEnumerable<IListProvider> listProviders)
    {
        _dialogService = dialogService;
        AddNewPageCommand = ReactiveCommand.Create(AddNewPage);
        RemoveCommand = ReactiveCommand.Create(Remove);
        MakeListCommand = ReactiveCommand.CreateFromTask(MakeList);
        
        ListProviders = listProviders.ToObservableCollection();
        SelectedListProvider = ListProviders[0];
    }

    private async Task MakeList(CancellationToken arg)
    {
        if (SelectedListProvider.NeedsAdditionalParams)
        {
            await SelectedListProvider.GetAdditionalParams();
        }
        
        if (!SelectedListProvider.CanMake)
        {
            return;
        }

        Result<List<string>> result = await SelectedListProvider.MakeList();
        if (result is {IsSuccessful: true, Value: not null})
        {
            Pages.AddRange(result.Value);
        }
        else
        {
            await _dialogService.Alert("Failed to get the list", result.Error ?? "Failed to get the list. Please make sure you have internet access.");
        }
    }

    private void Remove()
    {
        if (SelectedPages.Any())
        {
            Pages.Remove(new List<string>(SelectedPages));
        }

        SelectedPages = new ObservableCollection<string>();
    }

    private void AddNewPage()
    {
        if (!string.IsNullOrWhiteSpace(NewPageTitle) && !Pages.Contains(NewPageTitle.Trim()))
        {
            Pages.Add(NewPageTitle.Trim());
        }

        NewPageTitle = string.Empty;
    }

    public ObservableCollection<IListProvider> ListProviders { get; }
    [Reactive] public IListProvider SelectedListProvider { get; set; }
    [Reactive] public ObservableCollection<string> Pages { get; set; } = new();
    [Reactive] public ObservableCollection<string> SelectedPages { get; set; } = new();
    [Reactive] public string NewPageTitle { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> AddNewPageCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; set; }
    public ReactiveCommand<Unit, Unit> MakeListCommand { get; set; }
}