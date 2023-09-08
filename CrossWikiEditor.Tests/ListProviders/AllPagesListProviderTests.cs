using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Tests.ListProviders;

public class AllPagesListProviderTests : ListProvidersBaseTest
{
    private AllPagesListProvider _sut;
    private SelectNamespacesViewModel _selectNamespacesViewModel;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _selectNamespacesViewModel = new SelectNamespacesViewModel(new List<WikiNamespace>(), false);
        _sut = new AllPagesListProvider(_dialogService, _pageService, _viewModelFactory, _userPreferencesService)
        {
            Param = "start from here"
        };
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(new[] {7, 2, 3, 9});
        _viewModelFactory.GetSelectNamespacesViewModel(false).Returns(_selectNamespacesViewModel);
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        // arrange

        // act

        // assert
        await _dialogService.DidNotReceive().ShowDialog<int[]?>(Arg.Any<SelectNamespacesViewModel>());
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList()
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(new int[] {});

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    [Test]
    public async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList()
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(_selectNamespacesViewModel).Returns(new int[] {1});

        // act
        await _sut.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeTrue();
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        List<WikiPageModel>? expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
        _pageService.GetAllPages(_userPrefs.UrlApi(), _sut.Param, 7, PropertyFilterOption.Disable, PropertyFilterOption.Disable, 73)
            .Returns(Result<List<WikiPageModel>>.Success(expectedPages));

        // act
        await _sut.GetAdditionalParams();
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
        _pageService.GetAllPages(_userPrefs.UrlApi(), _sut.Param, 7, PropertyFilterOption.Disable, PropertyFilterOption.Disable, 73)
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
        _sut.Title.Should().Be("All Pages");
        _sut.ParamTitle.Should().Be("Start from");
    }
}