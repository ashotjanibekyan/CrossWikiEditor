namespace CrossWikiEditor.Tests.ListProviders;

public sealed class NewPagesListProviderTests : ListProvidersBaseTest<NewPagesListProvider>
{
    private readonly int[] _testArray = [7, 2, 3, 9];

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel([], false);
        _sut = new NewPagesListProvider(_dialogService, _pageService, _settingsService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(_testArray);
        _viewModelFactory.GetSelectNamespacesViewModel().Returns(_selectNamespacesViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public new async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        await base.CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList()
    {
        await base.CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList(_selectNamespacesViewModel);
    }

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList()
    {
        await base.CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList(_selectNamespacesViewModel);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetNewPages(_userSettings.GetApiUrl(), Arg.Is<int[]>(x => x.SequenceEqual(_testArray)), 73)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetNewPages(_userSettings.GetApiUrl(), Arg.Is<int[]>(x => x.SequenceEqual(_testArray)), 73)
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
        _sut.Title.Should().Be("New pages");
        _sut.ParamTitle.Should().Be("");
    }
}