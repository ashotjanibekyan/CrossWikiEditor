namespace CrossWikiEditor.Tests.ListProviders;

public abstract class ListProvidersBaseTest<T> : BaseTest where T : ListProviderBase
{
    protected UserSettings _userSettings;
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

    protected void SetUpUserSettings(string languageCode, ProjectEnum project)
    {
        _userSettings = new UserSettings
        {
            UserWiki = new(languageCode, project)
        };
        _settingsService.GetCurrentSettings().Returns(_userSettings);
        _settingsService.CurrentApiUrl.Returns(_userSettings.GetApiUrl());
    }

    protected async Task MakeList_ShouldReturnServiceResults(List<WikiPageModel> expectedPages)
    {
        // arrange

        // act
        if (_sut is INeedAdditionalParamsListProvider needAdditionalParamsListProvider)
        {
            await needAdditionalParamsListProvider.GetAdditionalParams();
        }
        Result<List<WikiPageModel>> result = await (_sut as ILimitedListProvider)!.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedPages);
    }

    protected void CanMake_ShouldBeFalse_WhenParamIsEmpty()
    {
        // arrange
        _sut.Param = "";

        // act
        var result = _sut.CanMake;

        // assert
        result.Should().BeFalse();
    }

    protected void CanMake_ShouldBeTrue_WhenParamIsEmpty()
    {
        // arrange
        _sut.Param = "not empty";

        // act
        var result = _sut.CanMake;

        // assert
        result.Should().BeTrue();
    }
    
    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsNotCalled()
    {
        // arrange

        // act

        // assert
        await _dialogService.DidNotReceive().ShowDialog<int[]?>(Arg.Any<SelectNamespacesViewModel>());
        _sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeFalse_WhenGetAdditionalParamsReturnsEmptyList(SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new int[] {});

        // act
        await (_sut as INeedAdditionalParamsListProvider)!.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeFalse();
    }

    protected async Task CanMake_ShouldBeTrue_WhenGetAdditionalParamsReturnsNonEmptyList(SelectNamespacesViewModel selectNamespacesViewModel)
    {
        // arrange
        _dialogService.ShowDialog<int[]?>(selectNamespacesViewModel).Returns(new[] {1});

        // act
        await (_sut as INeedAdditionalParamsListProvider)!.GetAdditionalParams();

        // assert
        _sut.CanMake.Should().BeTrue();
    }
}