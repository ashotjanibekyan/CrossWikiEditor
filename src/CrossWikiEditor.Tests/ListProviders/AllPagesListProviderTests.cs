namespace CrossWikiEditor.Tests.ListProviders;

public sealed class AllPagesListProviderTests : ListProvidersBaseTest<AllPagesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel(new List<WikiNamespace>(), false);
        _sut = new AllPagesListProvider(_dialogService, _pageService, _userPreferencesService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(new[] {7, 2, 3, 9});
        _viewModelFactory.GetSelectNamespacesViewModel(false).Returns(_selectNamespacesViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public new async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled() =>
        await base.CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled();

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList() =>
        await base.CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList(_selectNamespacesViewModel);

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList() =>
        await base.CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList(_selectNamespacesViewModel);


    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetAllPages(_userSettings.GetApiUrl(), _sut.Param, 7, PropertyFilterOption.Disable, PropertyFilterOption.Disable, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetAllPages(_userSettings.GetApiUrl(), _sut.Param, 7, PropertyFilterOption.Disable, PropertyFilterOption.Disable, 73)
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
        _sut.Title.Should().Be("All Pages");
        _sut.ParamTitle.Should().Be("Start from");
    }
}