namespace CrossWikiEditor.Tests.ListProviders;

public sealed class AllPagesWithPrefixListProviderTests : ListProvidersBaseTest<AllPagesWithPrefixListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new AllPagesWithPrefixListProvider(_dialogService, _pageService, _settingsService, _viewModelFactory)
        {
            Param = "my prefix"
        };
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns([7, 2, 3, 9]);
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
        _pageService.GetAllPagesWithPrefix(_userSettings.GetApiUrl(), _sut.Param, 7, 73)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetAllPagesWithPrefix(_userSettings.GetApiUrl(), _sut.Param, 7, 73)
            .Returns(new Exception("failed to get pages"));

        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("failed to get pages");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("All Pages with prefix (Prefixindex)");
        _sut.ParamTitle.Should().Be("Prefix");
    }
}