using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Tests.ListProviders;

public class AllFilesListProviderTests : ListProvidersBaseTest
{
    private AllFilesListProvider _sut;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new AllFilesListProvider(_dialogService, _pageService, _userPreferencesService)
        {
            Param = "start from here"
        };
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        List<WikiPageModel>? expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
        _pageService.GetAllFiles(_userPrefs.UrlApi(), _sut.Param, 73).Returns(Result<List<WikiPageModel>>.Success(expectedPages));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetAllFiles(_userPrefs.UrlApi(), _sut.Param, 73).Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("failed to get pages");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.CanMake.Should().BeTrue();
        _sut.Title.Should().Be("All Files");
        _sut.ParamTitle.Should().Be("Start from");
    }
}