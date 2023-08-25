using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using CrossWikiEditor.PageProviders;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class MakeListViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public MakeListViewModel(IDialogService dialogService, IPageService pageService, IUserPreferencesService userPreferencesService)
    {
        _dialogService = dialogService;
        AddNewPageCommand = ReactiveCommand.Create(AddNewPage);
        RemovePageCommand = ReactiveCommand.Create(RemovePage);
        MakeListCommand = ReactiveCommand.CreateFromTask(MakeList);
        
        PageProviders = new List<IPageProvider>
        {
            new CategoriesOnPageProvider(pageService, userPreferencesService)
        }.ToObservableCollection();
        SelectedPageProvider = PageProviders[0];
    }

    private async Task MakeList(CancellationToken arg)
    {
        if (!SelectedPageProvider.CanMake)
        {
            return;
        }

        Result<List<string>> result = await SelectedPageProvider.MakeList();
        if (result is {IsSuccessful: true, Value: not null})
        {
            Pages.AddRange(result.Value);
        }
        else
        {
            await _dialogService.Alert("Failed to get the list", result.Error ?? "Failed to get the list. Please make sure you have internet access.");
        }
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

    public ObservableCollection<IPageProvider> PageProviders { get; }
    [Reactive] public IPageProvider SelectedPageProvider { get; set; }
    [Reactive] public ObservableCollection<string> Pages { get; set; } = new();
    [Reactive] public string SelectedPage { get; set; }
    [Reactive] public string NewPageTitle { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> AddNewPageCommand { get; }
    public ReactiveCommand<Unit, Unit> RemovePageCommand { get; set; }
    public ReactiveCommand<Unit, Unit> MakeListCommand { get; set; }
}