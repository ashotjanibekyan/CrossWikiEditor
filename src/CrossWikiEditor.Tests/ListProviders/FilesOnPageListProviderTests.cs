namespace CrossWikiEditor.Tests.ListProviders;

public sealed class FilesOnPageListProviderTests : ListProvidersBaseTest<FilesOnPageListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new FilesOnPageListProvider(_dialogService, _pageService, _userPreferencesService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();
    
    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _pageService.FilesOnPage(_userPreferencesService.CurrentApiUrl, _sut.Param, 73)
            .Returns(_expectedPages);

        await base.MakeList_ShouldReturnServiceResults(_expectedPages);
    }
    
    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.FilesOnPage(_userPreferencesService.CurrentApiUrl, _sut.Param, 73)
            .Returns(new Exception("failed to get pages"));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("failed to get pages");
    }
    
    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Files on page");
        _sut.ParamTitle.Should().Be("Files on");
    }
}