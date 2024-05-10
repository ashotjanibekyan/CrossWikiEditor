namespace CrossWikiEditor.Tests.ListProviders;

public sealed class NewPagesListProviderTests : ListProvidersBaseTest<NewPagesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel(new List<WikiNamespace>(), false);
        _sut = new NewPagesListProvider(_dialogService, _pageService, _userPreferencesService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(new[] {7, 2, 3, 9});
        _viewModelFactory.GetSelectNamespacesViewModel(true).Returns(_selectNamespacesViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
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
        _pageService.GetNewPages(_userPrefs.UrlApi(), Arg.Is<int[]>(x => x.SequenceEqual(new[] {7, 2, 3, 9})), 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetNewPages(_userPrefs.UrlApi(), Arg.Is<int[]>(x => x.SequenceEqual(new[] {7, 2, 3, 9})), 73)
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
        _sut.Title.Should().Be("New pages");
        _sut.ParamTitle.Should().Be("");
    }
}