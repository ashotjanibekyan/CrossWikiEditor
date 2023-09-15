namespace CrossWikiEditor.Tests.ListProviders;

public sealed class RandomListProviderTests : ListProvidersBaseTest<RandomListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesAndRedirectFilterViewModel = new SelectNamespacesAndRedirectFilterViewModel(new List<WikiNamespace>()
        {
            new(0, ""),
            new(1, "Քննարկում:")
        })
        {
            IsIncludeRedirectsVisible = false
        };
        _sut = new RandomListProvider(_dialogService, _pageService, _userPreferencesService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{3, 4}, true, RedirectFilter.All));
        _viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel(false).Returns(_selectNamespacesAndRedirectFilterViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test]
    public new async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        await _dialogService.DidNotReceive().ShowDialog<NamespacesAndRedirectFilterOptions>(Arg.Any<SelectNamespacesAndRedirectFilterViewModel>());
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenDialogReturnsNull()
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns((NamespacesAndRedirectFilterOptions) null);

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenDialogReturnsNotNull()
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{2}, true, RedirectFilter.All));

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeTrue();
    }
    
    [TestCase(RedirectFilter.All, null, true)]
    [TestCase(RedirectFilter.Redirects, true, true)]
    [TestCase(RedirectFilter.NoRedirects, false, true)]
    [TestCase((RedirectFilter)7, null, true)]
    [TestCase(RedirectFilter.All, null, false)]
    [TestCase(RedirectFilter.Redirects, true, false)]
    [TestCase(RedirectFilter.NoRedirects, false, false)]
    [TestCase((RedirectFilter)7, null, false)]
    public async Task MakeList_ShouldReturnPageServiceResults(RedirectFilter redirectFilter, bool? filterRedirects, bool allowRedirectLinks)
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{3, 4}, allowRedirectLinks, redirectFilter));
        _pageService.GetRandomPages(_userPrefs.UrlApi(), Arg.Is<int[]?>(x => x.SequenceEqual(new []{3, 4})), filterRedirects, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [TestCase(RedirectFilter.All, null, true)]
    [TestCase(RedirectFilter.Redirects, true, true)]
    [TestCase(RedirectFilter.NoRedirects, false, true)]
    [TestCase((RedirectFilter)7, null, true)]
    [TestCase(RedirectFilter.All, null, false)]
    [TestCase(RedirectFilter.Redirects, true, false)]
    [TestCase(RedirectFilter.NoRedirects, false, false)]
    [TestCase((RedirectFilter)7, null, false)]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult(RedirectFilter redirectFilter, bool? filterRedirects, bool allowRedirectLinks)
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{3, 4}, allowRedirectLinks, redirectFilter));
        _pageService.GetRandomPages(_userPrefs.UrlApi(), Arg.Is<int[]?>(x => x.SequenceEqual(new []{3, 4})), filterRedirects, 73)
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
        _sut.Title.Should().Be("Random pages");
        _sut.ParamTitle.Should().Be("");
    }
}