using System;
using Avalonia.Controls;
using CrossWikiEditor.Core;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Views;

public partial class DatabaseScannerView : Window, IDialog
{
    public DatabaseScannerView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is DatabaseScannerViewModel vm)
        {
            vm.ConvertedTextChanged += (sender, s) =>
            {
                TextEditor.Text = s;
            };
        }
    }
}