namespace CrossWikiEditor.Tests.ListProviders;

public class WhatLinksHereListProviderTests : ListProvidersBaseTest<WhatLinksHereListProvider>
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
        });
        _sut = new WhatLinksHereListProvider(_dialogService, _pageService, _userPreferencesService, _viewModelFactory)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{3, 4}, true, RedirectFilter.All));
        _viewModelFactory.GetSelectNamespacesAndRedirectFilterViewModel().Returns(_selectNamespacesAndRedirectFilterViewModel);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        await _dialogService.DidNotReceive().ShowDialog<NamespacesAndRedirectFilterOptions>(Arg.Any<SelectNamespacesAndRedirectFilterViewModel>());
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenParamIsNotEmptyAndDialogReturnsNull()
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns((NamespacesAndRedirectFilterOptions) null);
        _sut.Param = "not empty";

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenParamIsEmptyAndDialogReturnsNotNull()
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{2}, true, RedirectFilter.All));
        _sut.Param = "";

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenParamIsNotEmptyAndDialogReturnsNotNull()
    {
        // arrange
        _dialogService.ShowDialog<NamespacesAndRedirectFilterOptions>(_selectNamespacesAndRedirectFilterViewModel)
            .Returns(new NamespacesAndRedirectFilterOptions(new []{2}, true, RedirectFilter.All));
        _sut.Param = "not empty";

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
        _pageService.GetPagesLinkedTo(_userPrefs.UrlApi(), _sut.Param, Arg.Is<int[]?>(x => x.SequenceEqual(new []{3, 4})), allowRedirectLinks, filterRedirects, 73)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_sut, _expectedPages);
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
        _pageService.GetPagesLinkedTo(_userPrefs.UrlApi(), _sut.Param, Arg.Is<int[]?>(x => x.SequenceEqual(new []{3, 4})), allowRedirectLinks, filterRedirects, 73)
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
        _sut.Title.Should().Be("What links here");
        _sut.ParamTitle.Should().Be("What links to");
    }
}