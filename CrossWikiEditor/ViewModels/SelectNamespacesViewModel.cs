using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Reactive;
using CrossWikiEditor.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public class SelectNamespacesViewModel : ViewModelBase
{
    public SelectNamespacesViewModel(List<WikiNamespace> namespaces)
    {
        Namespaces = namespaces.Where(x => x.Id >= 0).ToObservableCollection();
        SelectCommand = ReactiveCommand.Create<IDialog>(Select);
        
        this.WhenAnyValue(x => x.IsAllSelected)
            .Subscribe((val) =>
            {
                Namespaces = Namespaces
                    .ToList()
                    .Select(x => new WikiNamespace(x.Id, x.Name, val))
                    .ToObservableCollection();
            });
    }

    private void Select(IDialog dialog)
    {
        dialog.Close(Result<int[]>.Success(Namespaces.Where(n => n.IsChecked).Select(n => n.Id).ToArray()));
    }
    
    [Reactive] public ObservableCollection<WikiNamespace> Namespaces { get; set; }
    [Reactive] public bool IsAllSelected { get; set; }
    public ReactiveCommand<IDialog, Unit> SelectCommand { get; }
}