using System;
using System.Reactive;
using ReactiveUI;

namespace CrossWikiEditor.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    public StatusBarViewModel()
    {
        UsernameClickedCommand = ReactiveCommand.Create(UsernameClicked);
    }
    
    public string Username { get; set; } = "User: ";
    public ReactiveCommand<Unit, Unit> UsernameClickedCommand { get; }

    private void UsernameClicked()
    {
        Console.WriteLine("Username is clicked");
    }
}