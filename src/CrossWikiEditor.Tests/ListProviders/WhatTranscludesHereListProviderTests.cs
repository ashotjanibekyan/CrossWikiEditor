namespace CrossWikiEditor.Tests.ListProviders;

public sealed class WhatTranscludesHereListProviderTests : ListProvidersBaseTest<WhatTranscludesHereListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel([], false);
        _sut = new WhatTranscludesHereListProvider(_dialogService, _pageService, _settingsService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test]
    public new void CanMake_ShouldBeFalse_WhenParamIsEmpty()
    {
        base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    }

    [Test]
    public new void CanMake_ShouldBeTrue_WhenParamIsEmpty()
    {
        base.CanMake_ShouldBeTrue_WhenParamIsEmpty();
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _pageService.GetTransclusionsOf(_userSettings.GetApiUrl(), _sut.Param, Arg.Is<int[]>(x => x[0] == 0), 73)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetTransclusionsOf(_userSettings.GetApiUrl(), _sut.Param, Arg.Is<int[]>(x => x[0] == 0), 73)
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
        _sut.Title.Should().Be("What transcludes page");
        _sut.ParamTitle.Should().Be("What embeds");
    }
}