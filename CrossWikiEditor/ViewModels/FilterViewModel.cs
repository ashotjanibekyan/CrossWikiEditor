using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public class WikiNamespace
{
    public WikiNamespace(int id, string name, bool isChecked = false)
    {
        Id = id;
        Name = name;
        IsChecked = isChecked;
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public bool IsChecked { get; set; }
}

public class FilterViewModel : ViewModelBase
{
    private readonly TextFileListProvider _textFileListProvider;
    
    public FilterViewModel(List<WikiNamespace> subjectNamespaces, List<WikiNamespace> talkNamespaces, TextFileListProvider textFileListProvider)
    {
        _textFileListProvider = textFileListProvider;
        this.WhenAnyValue(x => x.IsAllTalkChecked)
            .Subscribe((val) =>
            {
                if (TalkNamespaces is null)
                {
                    return;
                }
                TalkNamespaces = TalkNamespaces
                    .ToList()
                    .Select(x => new WikiNamespace(x.Id, x.Name, val))
                    .ToObservableCollection();
            });
        
        this.WhenAnyValue(x => x.IsAllSubjectChecked)
            .Subscribe((val) =>
            {
                if (SubjectNamespaces is null)
                {
                    return;
                }
                SubjectNamespaces = SubjectNamespaces
                    .ToList()
                    .Select(x => new WikiNamespace(x.Id, x.Name, val))
                    .ToObservableCollection();
            });
        
        SubjectNamespaces = subjectNamespaces.ToObservableCollection();
        TalkNamespaces = talkNamespaces.ToObservableCollection();
        
        SaveCommand = ReactiveCommand.Create<IDialog>(Save);
        CloseCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(Result<FilterOptions>.Failure("Closed without selecting any value")));
        OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFile);
        ClearCommand = ReactiveCommand.Create(Clear);
    }

    private void Save(IDialog dialog)
    {
        IEnumerable<int> arr1 = SubjectNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        IEnumerable<int> arr2 = TalkNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
        dialog.Close(Result<FilterOptions>.Success(new FilterOptions(
            arr1.Concat(arr2).ToArray(), 
            RemoveTitlesContaining,
            KeepTitlesContaining,
            UseRegex,
            SortAlphabetically,
            RemoveDuplicates)));
    }

    private async Task OpenFile()
    {
        if (_textFileListProvider.NeedsAdditionalParams)
        {
            await _textFileListProvider.GetAdditionalParams();
        }

        if (_textFileListProvider.CanMake)
        {
            Result<List<WikiPageModel>> result = await _textFileListProvider.MakeList();
            if (result is {IsSuccessful: true, Value: not null})
            {
                Pages = result.Value.ToObservableCollection();
            }
        }
    }
    
    private void Clear()
    {
        Pages.Clear();
    }

    [Reactive] public ObservableCollection<WikiNamespace> SubjectNamespaces { get; set; }
    [Reactive] public ObservableCollection<WikiNamespace> TalkNamespaces { get; set; }
    [Reactive] public ObservableCollection<WikiPageModel> Pages { get; set; }
    [Reactive] public bool IsAllTalkChecked { get; set; }
    [Reactive] public bool IsAllSubjectChecked { get; set; }
    [Reactive] public bool UseRegex { get; set; }
    [Reactive] public bool SortAlphabetically { get; set; }
    [Reactive] public bool RemoveDuplicates { get; set; }
    [Reactive] public string RemoveTitlesContaining { get; set; } = string.Empty;
    [Reactive] public string KeepTitlesContaining { get; set; } = string.Empty;
    public ReactiveCommand<IDialog, Unit> SaveCommand { get; }
    public ReactiveCommand<IDialog, Unit> CloseCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
}