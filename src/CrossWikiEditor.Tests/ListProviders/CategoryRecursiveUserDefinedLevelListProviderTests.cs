namespace CrossWikiEditor.Tests.ListProviders;

public sealed class CategoryRecursiveUserDefinedLevelListProviderTests : ListProvidersBaseTest<CategoryRecursiveUserDefinedLevelListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserSettings("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoryRecursiveUserDefinedLevelListProvider(_categoryService, _dialogService, _settingsService)
        {
            Param = "my prefix"
        };
        _dialogService.ShowDialog<int?>(Arg.Is<PromptViewModel>(vm => vm.IsNumeric && vm.Text == "Recursion depth: "))
            .Returns(12);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userSettings.GetApiUrl(), _wikiClientCache).Generate(4);
    }

    [TestCase("", true, ExpectedResult = false)]
    [TestCase("", false, ExpectedResult = false)]
    [TestCase("not empty", false, ExpectedResult = false)]
    [TestCase("not empty", true, ExpectedResult = true)]
    public async Task<bool> CanMake_ShouldDependOn_RecursionLevelAndParam(string param, bool recLevelSelected)
    {
        // arrange
        _sut.Param = param;
        if (recLevelSelected)
        {
            _dialogService.ShowDialog<int?>(Arg.Is<PromptViewModel>(vm => vm.IsNumeric && vm.Text == "Recursion depth: "))
                .Returns(12);
        }
        else
        {
            _dialogService.ShowDialog<int?>(Arg.Is<PromptViewModel>(vm => vm.IsNumeric && vm.Text == "Recursion depth: "))
                .Returns((int?) null);
        }

        // act
        await _sut.GetAdditionalParams();

        // assert
        return _sut.CanMake;
    }

    [Test]
    public async Task MakeList_ShouldReturnPageServiceResults()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userSettings.GetApiUrl(), _sut.Param, 73, 12)
            .Returns(_expectedPages);

        await MakeList_ShouldReturnServiceResults(_expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userSettings.GetApiUrl(), _sut.Param, 73, 12)
            .Returns(new Exception("failed to get pages"));

        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("failed to get pages");
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenRecursionLevelIsNotSelected()
    {
        // arrange

        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Be("Please select recursive level.");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Category (recursive user defined level)");
        _sut.ParamTitle.Should().Be("Category");
    }
}