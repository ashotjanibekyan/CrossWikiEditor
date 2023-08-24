using CrossWikiEditor.Repositories;
using CrossWikiEditor.Services;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;
using NSubstitute;

namespace CrossWikiEditor.Tests.ViewModels;

public class ProfilesViewModelTests : BaseTest
{
    private ProfilesViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new ProfilesViewModel(_fileDialogService, _dialogService, _profileRepository, _userService, _userPreferencesServic);
    }
}