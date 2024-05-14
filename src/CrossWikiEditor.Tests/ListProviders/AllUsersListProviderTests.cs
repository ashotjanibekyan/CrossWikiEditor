namespace CrossWikiEditor.Tests.ListProviders;

public sealed class AllUsersListProviderTests : ListProvidersBaseTest<AllUsersListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new AllUsersListProvider(_dialogService, _settingsService, _userService);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [Test] public new void CanMake_ShouldBeFalse_WhenParamIsEmpty() => base.CanMake_ShouldBeFalse_WhenParamIsEmpty();
    [Test] public new void CanMake_ShouldBeTrue_WhenParamIsEmpty() => base.CanMake_ShouldBeTrue_WhenParamIsEmpty();

    [Test]
    public async Task MakeList_ShouldReturnServiceResults()
    {
        // arrange
        _userService.GetAllUsers(_settingsService.CurrentApiUrl, _sut.Param, 73)
            .Returns(_expectedPages);

        await base.MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _userService.GetAllUsers(_settingsService.CurrentApiUrl, _sut.Param, 73)
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
        _sut.Title.Should().Be("All Users");
        _sut.ParamTitle.Should().Be("Start from");
    }
}