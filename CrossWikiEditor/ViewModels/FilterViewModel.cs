using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ViewModels;

public class WikiNamespace(int id, string name, bool isChecked = false)
{
    public int Id { get; init; } = id;
    public string Name { get; init; } = name;
    public bool IsChecked { get; set; } = isChecked;
}

public partial class FilterViewModel(List<WikiNamespace> subjectNamespaces, List<WikiNamespace> talkNamespaces,
        TextFileListProvider textFileListProvider)
    : ViewModelBase
{
    [RelayCommand]
    private void Save(IDialog dialog)
    {
        IEnumerable<int> arr1 = SubjectNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        IEnumerable<int> arr2 = TalkNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        dialog.Close(new FilterOptions(
            arr1.Concat(arr2).ToArray(),
            RemoveTitlesContaining,
            KeepTitlesContaining,
            UseRegex,
            SortAlphabetically,
            RemoveDuplicates,
            SelectedSetOperations,
            Pages.ToList()));
    }

    [RelayCommand]
    private void Close(IDialog dialog)
    {
        dialog.Close(null);
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        await textFileListProvider.GetAdditionalParams();

        if (textFileListProvider.CanMake)
        {
            Result<List<WikiPageModel>> result = await textFileListProvider.MakeList();
            if (result is {IsSuccessful: true, Value: not null})
            {
                Pages = result.Value.ToObservableCollection();
            }
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Pages.Clear();
    }

    partial void OnIsAllTalkCheckedChanged(bool value)
    {
        TalkNamespaces = TalkNamespaces
            .ToList()
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }

    partial void OnIsAllSubjectCheckedChanged(bool value)
    {
        SubjectNamespaces = SubjectNamespaces
            .ToList()
            .Select(x => new WikiNamespace(x.Id, x.Name, value))
            .ToObservableCollection();
    }

    [ObservableProperty] private ObservableCollection<WikiNamespace> _subjectNamespaces = subjectNamespaces.ToObservableCollection();
    [ObservableProperty] private ObservableCollection<WikiNamespace> _talkNamespaces = talkNamespaces.ToObservableCollection();
    [ObservableProperty] private ObservableCollection<WikiPageModel> _pages = new();

    [ObservableProperty] private ObservableCollection<SetOperations> _setOperations =
        new[] {Models.SetOperations.SymmetricDifference, Models.SetOperations.Intersection}.ToObservableCollection();

    [ObservableProperty] private bool _isAllTalkChecked;
    [ObservableProperty] private bool _isAllSubjectChecked;
    [ObservableProperty] private bool _useRegex;
    [ObservableProperty] private bool _sortAlphabetically;
    [ObservableProperty] private bool _removeDuplicates;
    [ObservableProperty] private string _removeTitlesContaining = string.Empty;
    [ObservableProperty] private string _keepTitlesContaining = string.Empty;
    [ObservableProperty] private SetOperations _selectedSetOperations = Models.SetOperations.SymmetricDifference;
}