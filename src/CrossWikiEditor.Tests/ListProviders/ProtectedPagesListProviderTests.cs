namespace CrossWikiEditor.Tests.ListProviders;

public sealed class ProtectedPagesListProviderTests : ListProvidersBaseTest<ProtectedPagesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _selectProtectionSelectionPageViewModel = new SelectProtectionSelectionPageViewModel();
        _sut = new ProtectedPagesListProvider(_dialogService, _pageService, _userPreferencesService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<(string, string)>(_selectProtectionSelectionPageViewModel).Returns(("edit", "autoconfirmed"));
        _viewModelFactory.GetSelectProtectionSelectionPageViewModel().Returns(_selectProtectionSelectionPageViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public new async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        await _dialogService.DidNotReceive().ShowDialog<(string, string)>(Arg.Any<SelectProtectionSelectionPageViewModel>());
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyTuple()
    {
        // arrange
        _dialogService.ShowDialog<(string, string)>(_selectProtectionSelectionPageViewModel).Returns(("", ""));

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyTuple()
    {
        // arrange
        _dialogService.ShowDialog<(string, string)>(_selectProtectionSelectionPageViewModel).Returns(("edit", "sysop"));

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeTrue();
    }
    
    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetProtectedPages(_userSettings.GetApiUrl(), "edit", "autoconfirmed", 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetProtectedPages(_userSettings.GetApiUrl(), "edit", "autoconfirmed", 73)
            .Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("failed to get pages");
    }
    
    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Protected pages");
        _sut.ParamTitle.Should().Be("");
    }
}