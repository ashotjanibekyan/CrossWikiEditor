using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
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

    public FilterViewModel(List<WikiNamespace> subjectNamespaces, List<WikiNamespace> talkNamespaces)
    {
        this.WhenAnyValue(x => x.IsAllTalkChecked)
            .Subscribe((val) =>
            {
                TalkNamespaces = TalkNamespaces
                    .ToList()
                    .Select(x => new WikiNamespace(x.Id, x.Name, val))
                    .ToObservableCollection();
            });
        
        this.WhenAnyValue(x => x.IsAllSubjectChecked)
            .Subscribe((val) =>
            {
                SubjectNamespaces = SubjectNamespaces
                    .ToList()
                    .Select(x => new WikiNamespace(x.Id, x.Name, val))
                    .ToObservableCollection();
            });
        
        SubjectNamespaces = subjectNamespaces.ToObservableCollection();
        TalkNamespaces = talkNamespaces.ToObservableCollection();
        
        SaveCommand = ReactiveCommand.Create((IDialog dialog) =>
        {
            var arr1 = SubjectNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
            var arr2 = TalkNamespaces.ToList().Where(x => x.IsChecked).Select(x => x.Id);
            dialog.Close(Result<FilterOptions>.Success(new FilterOptions(
                arr1.Concat(arr2).ToArray(), 
                RemoveTitlesContaining,
                KeepTitlesContaining,
                UseRegex,
                SortAlphabetically,
                RemoveDuplicates)));
        });

        CloseCommand = ReactiveCommand.Create((IDialog dialog) => dialog.Close(Result<FilterOptions>.Failure("Closed without selecting any value")));
    }

    [Reactive] public ObservableCollection<WikiNamespace> SubjectNamespaces { get; set; }
    [Reactive] public ObservableCollection<WikiNamespace> TalkNamespaces { get; set; }
    [Reactive] public bool IsAllTalkChecked { get; set; }
    [Reactive] public bool IsAllSubjectChecked { get; set; }
    [Reactive] public bool UseRegex { get; set; }
    [Reactive] public bool SortAlphabetically { get; set; }
    [Reactive] public bool RemoveDuplicates { get; set; }
    [Reactive] public string RemoveTitlesContaining { get; set; } = string.Empty;
    [Reactive] public string KeepTitlesContaining { get; set; } = string.Empty;
    public ReactiveCommand<IDialog, Unit> SaveCommand { get; }
    public ReactiveCommand<IDialog, Unit> CloseCommand { get; }
}