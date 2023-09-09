using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Settings;
using CrossWikiEditor.Core.Utils;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Tests.ListProviders;

public class ListProvidersBaseTest : BaseTest
{
    protected UserPrefs _userPrefs;
    protected SelectNamespacesViewModel _selectNamespacesViewModel;
    protected List<WikiPageModel> _expectedPages;

    protected void SetUpUserPrefs(string languageCode, ProjectEnum project)
    {
        _userPrefs = new UserPrefs
        {
            LanguageCode = languageCode,
            Project = project
        };
        _userPreferencesService.GetCurrentPref().Returns(_userPrefs);
    }

    protected async Task MakeList_ShouldReturnPageServiceResults(LimitedListProviderBase sut, List<WikiPageModel> expectedPages)
    {
        // arrange

        // act
        if (sut is INeedNamespacesListProvider needNamespacesListProvider)
        {
            await needNamespacesListProvider.GetAdditionalParams();
        }
        Result<List<WikiPageModel>> result = await sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedPages);
    }
    
    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled(INeedNamespacesListProvider sut)
    {
        // arrange

        // act

        // assert
        await _dialogService.DidNotReceive().ShowDialog<int[]?>(Arg.Any<SelectNamespacesViewModel>());
        sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList(INeedNamespacesListProvider sut, SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new int[] {});

        // act
        await sut.GetAdditionalParams();

        // assert
        sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList(INeedNamespacesListProvider sut, SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new[] {1});

        // act
        await sut.GetAdditionalParams();

        // assert
        sut.CanMake.Should().BeTrue();
    }
}