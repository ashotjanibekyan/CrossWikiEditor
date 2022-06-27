using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace AutoWikiEditor.ViewModels;

public class MenuViewModel
{
    public MenuViewModel()
    {
        this.ExitCommand = ReactiveCommand.Create<Window>(this.Exit);
    }
    public ReactiveCommand<Window, Unit> ExitCommand { get; }
    
    private void Exit(Window window) 
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime controlledApplicationLifetime)
        {
            controlledApplicationLifetime.Shutdown();
        }
        else
        {
            Environment.Exit(1);
        }
    }
}