using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Tests.ListProviders;

public class AllCategoriesListProviderTests : BaseTest
{
    private AllCategoriesListProvider _sut;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new AllCategoriesListProvider(_dialogService, _pageService, _userPreferencesService);
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        var expectedPages = new List<WikiPageModel>
        {
            new("title", "https://hyw.wikipedia.org/w/api.php?", _wikiClientCache),
            new("title1", "https://hyw.wikipedia.org/w/api.php?", _wikiClientCache),
        };
        _pageService.GetAllCategories("https://hyw.wikipedia.org/w/api.php?", "", 73)
            .Returns(Result<List<WikiPageModel>>.Success(expectedPages));
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value[0].Title.Should().Be("title");
        result.Value[1].Title.Should().Be("title1");
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _pageService.GetAllCategories("https://hyw.wikipedia.org/w/api.php?", "", 73)
            .Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hyw",
            Project = ProjectEnum.Wikipedia
        });

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
        _sut.Title.Should().Be("All Categories");
        _sut.ParamTitle.Should().Be("Start from");
    }
}