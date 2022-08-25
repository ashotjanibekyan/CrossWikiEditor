namespace AutoWikiEditor.ViewModels;

public class ProfilesWindowViewModel : ViewModelBase, IDialogViewModel<object>
{
    public ProfilesWindowViewModel()
    {
        this.Result = (object)42;
    }

    public object Result { get; set; }
}