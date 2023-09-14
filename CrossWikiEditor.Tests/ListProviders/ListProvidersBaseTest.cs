namespace CrossWikiEditor.Tests.ListProviders;

public class ListProvidersBaseTest<T> : BaseTest where T : ListProviderBase
{
    protected UserPrefs _userPrefs;
    protected SelectNamespacesViewModel _selectNamespacesViewModel;
    protected SelectProtectionSelectionPageViewModel _selectProtectionSelectionPageViewModel;
    protected SelectNamespacesAndRedirectFilterViewModel _selectNamespacesAndRedirectFilterViewModel;
    protected List<WikiPageModel> _expectedPages;
    protected T _sut;

    [Test]
    public async Task GetLimit_ShouldReturn50_WhenNoValueIsReturned()
    {
        if (_sut is not ILimitedListProvider limitedListProvider)
        {
            Assert.Pass();
            return;
        }
        // arrange
        _dialogService.ShowDialog<int?>(Arg.Any<PromptViewModel>()).Returns(null as int?);

        // act
        var result = await limitedListProvider.GetLimit();

        // assert
        result.Should().Be(50);
    }

    [Test]
    public async Task GetLimit_ShouldReturnPromptValue()
    {
        if (_sut is not ILimitedListProvider limitedListProvider)
        {
            Assert.Pass();
            return;
        }
        // arrange
        _dialogService.ShowDialog<int?>(Arg.Is<PromptViewModel>(vm =>
            vm.IsNumeric && vm.Value == 50 && vm.Title == "How many page" && vm.Text == "Limit: ")).Returns(42);

        // act
        var result = await limitedListProvider.GetLimit();

        // assert
        result.Should().Be(42);
    }

    protected void SetUpUserPrefs(string languageCode, ProjectEnum project)
    {
        _userPrefs = new UserPrefs
        {
            LanguageCode = languageCode,
            Project = project
        };
        _userPreferencesService.GetCurrentPref().Returns(_userPrefs);
        _userPreferencesService.CurrentApiUrl.Returns(_userPrefs.UrlApi());
    }

    protected async Task MakeList_ShouldReturnServiceResults(LimitedListProviderBase sut, List<WikiPageModel> expectedPages)
    {
        // arrange

        // act
        if (sut is INeedAdditionalParamsListProvider needAdditionalParamsListProvider)
        {
            await needAdditionalParamsListProvider.GetAdditionalParams();
        }
        Result<List<WikiPageModel>> result = await sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedPages);
    }

    protected void CanMake_ShouldBeFalse_WhenParamIsEmpty(ListProviderBase sut)
    {
        // arrange
        sut.Param = "";

        // act
        var result = sut.CanMake;

        // assert
        result.Should().BeFalse();
    }

    protected void CanMake_ShouldBeTrue_WhenParamIsEmpty(ListProviderBase sut)
    {
        // arrange
        sut.Param = "not empty";

        // act
        var result = sut.CanMake;

        // assert
        result.Should().BeTrue();
    }
    
    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled(INeedAdditionalParamsListProvider sut)
    {
        // arrange

        // act

        // assert
        await _dialogService.DidNotReceive().ShowDialog<int[]?>(Arg.Any<SelectNamespacesViewModel>());
        sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList(INeedAdditionalParamsListProvider sut, SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new int[] {});

        // act
        await sut.GetAdditionalParams();

        // assert
        sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList(INeedAdditionalParamsListProvider sut, SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new[] {1});

        // act
        await sut.GetAdditionalParams();

        // assert
        sut.CanMake.Should().BeTrue();
    }
}