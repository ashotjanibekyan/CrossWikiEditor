namespace CrossWikiEditor.Tests.ListProviders;

public sealed class DisambiguationPagesListProviderTests : ListProvidersBaseTest<DisambiguationPagesListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new DisambiguationPagesListProvider(_dialogService, _pageService, _userPreferencesService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetPagesWithProp(_userSettings.GetApiUrl(), "disambiguation", 73)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetPagesWithProp(_userSettings.GetApiUrl(), "disambiguation", 73).Returns(new Exception("failed to get pages"));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("failed to get pages");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.CanMake.Should().BeTrue();
        _sut.Title.Should().Be("Disambiguation pages");
        _sut.ParamTitle.Should().Be("");
    }
}