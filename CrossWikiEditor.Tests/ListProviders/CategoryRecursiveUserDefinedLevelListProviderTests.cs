namespace CrossWikiEditor.Tests.ListProviders;

public class CategoryRecursiveUserDefinedLevelListProviderTests : ListProvidersBaseTest<CategoryRecursiveUserDefinedLevelListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        SetUpUserPrefs("hyw", ProjectEnum.Wikipedia);
        _sut = new CategoryRecursiveUserDefinedLevelListProvider(_categoryService, _dialogService, _userPreferencesService)
        {
            Param = "my prefix"
        };
        _dialogService.ShowDialog<int?>(Arg.Is<PromptViewModel>(vm => vm.IsNumeric && vm.Text == "Recursion depth: "))
            .Returns(12);
        _expectedPages = Fakers.GetWikiPageModelFaker(_userPrefs.UrlApi(), _wikiClientCache).Generate(4);
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
                .Returns((int?)null);
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
        _categoryService.GetPagesOfCategory(_userPrefs.UrlApi(), _sut.Param, 73, 12)
            .Returns(Result<List<WikiPageModel>>.Success(_expectedPages));

        await MakeList_ShouldReturnServiceResults(_sut, _expectedPages);
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenPageServiceReturnsUnsuccessfulResult()
    {
        // arrange
        _categoryService.GetPagesOfCategory(_userPrefs.UrlApi(), _sut.Param, 73, 12)
            .Returns(Result<List<WikiPageModel>>.Failure("failed to get pages"));

        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("failed to get pages");
    }

    [Test]
    public async Task MakeList_ShouldReturnUnsuccessfulResult_WhenRecursionLevelIsNotSelected()
    {
        // arrange
        
        // act
        Result<List<WikiPageModel>> result = await _sut.MakeList(73);

        // assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be("Please select recursive level.");
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Category (recursive user defined level)");
        _sut.ParamTitle.Should().Be("Category");
    }
}