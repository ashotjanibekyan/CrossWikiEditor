using CrossWikiEditor.Core.Services.WikiServices;
using Serilog;

namespace CrossWikiEditor.Tests.ListProviders;

public sealed class LinksOnPageRedListProviderTests : ListProvidersBaseTest<LinksOnPageRedListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _wikiClientCache = new WikiClientCache(Substitute.For<ILogger>());
        _selectNamespacesViewModel = new SelectNamespacesViewModel([], false);
        _sut = new LinksOnPageRedListProvider(_dialogService, _pageService, _settingsService)
        {
            Param = "start from here"
        };
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
        _expectedPages.Add(new WikiPageModel("Գրիգոր Ամիրեանի Բնակելի Տուն", _userSettings.GetApiUrl(), _wikiClientCache));
        _expectedPages.Add(new WikiPageModel("8 Յուլիս", _userSettings.GetApiUrl(), _wikiClientCache));
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
    public async Task MakeList_ShouldReturnBluePageServiceResults()
    {
        // arrange
        _pageService.LinksOnPage(_userSettings.GetApiUrl(), _sut.Param, 73)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(
        [
            _expectedPages[0],
            _expectedPages[1],
            _expectedPages[2],
            _expectedPages[3]
        ]);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.LinksOnPage(_userSettings.GetApiUrl(), _sut.Param, 73)
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
        _sut.Title.Should().Be("Links on page (only redlinks)");
        _sut.ParamTitle.Should().Be("Links on");
    }
}