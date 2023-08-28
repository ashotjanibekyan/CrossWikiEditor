using System.Collections.ObjectModel;
using CrossWikiEditor.ListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public class MakeListViewModelTests : BaseTest
{
    private MakeListViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        List<IListProvider> listProviders = new()
        {
            Substitute.For<IListProvider>(),
            Substitute.For<IListProvider>()
        };
        _sut = new MakeListViewModel(_dialogService, _wikiClientCache, _pageService, _systemService, _viewModelFactory, _fileDialogService,
            _userPreferencesService, listProviders);
    }

    [Test]
    public void AddNewPageCommand_ShouldDoNothing_WhenNewPageTitleIsEmpty([Values("", "  ", null)] string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;
        var originalPages = new List<WikiPageModel> {new("Page1", 0), new("Page2", 0), new("Page3", 0)};
        _sut.Pages = originalPages.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(originalPages);
    }

    [Test]
    public void AddNewPageCommand_ShouldAddPageToTheList_WhenNewPageTitleIsNotEmpty()
    {
        // arrange
        _sut.NewPageTitle = "new page";
        List<WikiPageModel>? originalPages = Fakers.WikiPageModelFaker.Generate(3);
        _sut.Pages = originalPages.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(originalPages.Concat(new[] {new WikiPageModel("new page", 0)}));
    }

    [Test]
    public void AddNewPageCommand_ShouldClearNewPageTitle([Values("", null, " ", "new title")] string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;

        // act
        _sut.AddNewPageCommand.Execute(null);

        // assert
        _sut.NewPageTitle.Should().BeEmpty();
    }

    [Test]
    public void AddNewPageCommand_ShouldTrimPage_WhenNewPageTitleContainsWhitespaces()
    {
        // arrange
        _sut.NewPageTitle = "    new page title   ";

        // act
        _sut.AddNewPageCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("new page title", 0)});
    }

    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        List<WikiPageModel>? randomPages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemoveCommand_ShouldRemoveSelectedPages_WhenSelectedPageIsInPages()
    {
        // arrange
        List<WikiPageModel>? randomPages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new[] {_sut.Pages[3], _sut.Pages[6], _sut.Pages[1]}.ToObservableCollection();
        randomPages.RemoveAt(1);
        randomPages.RemoveAt(2);
        randomPages.RemoveAt(4);

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenSelectedPageIsNotInPages()
    {
        List<WikiPageModel>? randomPages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = Fakers.WikiPageModelFaker.Generate(2).ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }


    [Test]
    public void RemoveCommand_ShouldClearSelectedPage()
    {
        // arrange
        _sut.SelectedPages = Fakers.WikiPageModelFaker.Generate(3).ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void MakeListCommand_ShouldDoNothing_WhenSelectedProviderCanNotMake()
    {
        // arrange
        IListProvider listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.CanMake.Returns(false);

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        listProvider.Received(0).MakeList();
        _dialogService.Received(0).Alert(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void MakeListCommand_ShouldUpdatePages_WhenMakeListReturnsNewList()
    {
        // arrange
        IListProvider listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;

        List<WikiPageModel>? existingPages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = existingPages.ToObservableCollection();

        listProvider.CanMake.Returns(true);

        List<WikiPageModel>? newPages = Fakers.WikiPageModelFaker.Generate(10);
        listProvider.MakeList().Returns(Result<List<WikiPageModel>>.Success(newPages));

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(existingPages.Concat(newPages));
    }

    [Test]
    public void MakeListCommand_ShouldAlertUser_WhenMakeListReturnsFailure()
    {
        // arrange
        IListProvider listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.CanMake.Returns(true);
        listProvider.MakeList().Returns(Result<List<WikiPageModel>>.Failure("error message"));
        List<WikiPageModel>? existingPages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = existingPages.ToObservableCollection();

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        _dialogService.Received(1).Alert("Failed to get the list", "error message");
        _sut.Pages.Should().BeEquivalentTo(existingPages);
    }

    [Test]
    public void MakeListCommand_ShouldRequestAdditionalParams_WhenNeedsAdditionalParamsIsTrue()
    {
        // arrange
        IListProvider listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.NeedsAdditionalParams.Returns(true);

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        listProvider.Received(1).GetAdditionalParams();
    }

    [Test]
    public void MakeListCommand_ShouldNotRequestAdditionalParams_WhenNeedsAdditionalParamsIsFalse()
    {
        // arrange
        IListProvider listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.NeedsAdditionalParams.Returns(false);

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        listProvider.Received(0).GetAdditionalParams();
    }

    [Test]
    public void OpenInBrowserCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.OpenInBrowserCommand.Execute(null);

        // assert
        _systemService.Received(0).OpenLinkInBrowser(Arg.Any<string>());
    }

    [Test]
    public void OpenInBrowserCommand_ShouldOpenAllSelectedPages_WhenThereAreSelectedPages()
    {
        // arrange
        _sut.SelectedPages = Fakers.WikiPageModelFaker.Generate(4).ToObservableCollection();
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _systemService.OpenLinkInBrowser(Arg.Any<string>()).Returns(Result.Success());

        // act
        _sut.OpenInBrowserCommand.Execute(null);

        // assert
        Received.InOrder(() =>
        {
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[0].Title}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[1].Title}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[2].Title}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[3].Title}");
        });
    }


    [Test]
    public void OpenHistoryInBrowserCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.OpenHistoryInBrowserCommand.Execute(null);

        // assert
        _systemService.Received(0).OpenLinkInBrowser(Arg.Any<string>());
    }

    [Test]
    public void OpenHistoryInBrowserCommand_ShouldOpenAllSelectedPages_WhenThereAreSelectedPages()
    {
        // arrange
        _sut.SelectedPages = Fakers.WikiPageModelFaker.Generate(4).ToObservableCollection();
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _systemService.OpenLinkInBrowser(Arg.Any<string>()).Returns(Result.Success());

        // act
        _sut.OpenHistoryInBrowserCommand.Execute(null);

        // assert
        Received.InOrder(() =>
        {
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[0].Title}&action=history");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[1].Title}&action=history");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[2].Title}&action=history");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={_sut.SelectedPages[3].Title}&action=history");
        });
    }

    [Test]
    public void CutCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.CutCommand.Execute(null);

        // assert
        _systemService.DidNotReceive().SetClipboardTextAsync(Arg.Any<string>());
    }

    [Test]
    public void CutCommand_ShouldCutSelectedPageTitlesAndRemoveThem()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(5);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {_sut.Pages[2], _sut.Pages[4], _sut.Pages[1]}.ToObservableCollection();

        // act
        _sut.CutCommand.Execute(null);

        // assert
        _systemService.Received(1)
            .SetClipboardTextAsync($"{pages[2].Title}{Environment.NewLine}{pages[4].Title}{Environment.NewLine}{pages[1].Title}");
        _sut.SelectedPages.Should().BeEmpty();
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {pages[0], pages[3]}.ToObservableCollection());
    }


    [Test]
    public void CopyCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.CopyCommand.Execute(null);

        // assert
        _clipboard.DidNotReceive().SetTextAsync(Arg.Any<string>());
    }

    [Test]
    public void CopyCommand_ShouldCopySelectedPageTitles()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(10);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(3);
        _sut.SelectedPages = selectedPages.ToObservableCollection();
        _sut.Pages = pages.ToObservableCollection();

        // act
        _sut.CopyCommand.Execute(null);

        // assert
        _systemService.Received(1)
            .SetClipboardTextAsync(
                $"{selectedPages[0].Title}{Environment.NewLine}{selectedPages[1].Title}{Environment.NewLine}{selectedPages[2].Title}");
        _sut.SelectedPages.Should().BeEquivalentTo(selectedPages);
        _sut.Pages.Should().BeEquivalentTo(pages);
    }

    [Test]
    public void SelectAllCommand_ShouldSelectAllPages()
    {
        // arrange
        _sut.Pages = Fakers.WikiPageModelFaker.Generate(5).ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectAllCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.Pages.Should().BeEquivalentTo(_sut.SelectedPages);
    }

    [Test]
    public void SelectNoneCommand_ShouldUnSelectAllPages()
    {
        // arrange
        _sut.Pages = Fakers.WikiPageModelFaker.Generate(5).ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectNoneCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SelectInverseCommand_ShouldSelectAll_WhenNonIsSelected()
    {
        // arrange
        _sut.Pages = Fakers.WikiPageModelFaker.Generate(5).ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectInverseCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.Pages.Should().BeEquivalentTo(_sut.SelectedPages);
    }

    [Test]
    public void SelectInverseCommand_ShouldUnselectAll_WhenEverythingIsSelected()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(5);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = pages.ToObservableCollection();

        // act
        _sut.SelectInverseCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SelectInverseCommand_ShouldInverseSelection()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(5);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(2);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = selectedPages.ToObservableCollection();

        // act
        _sut.SelectInverseCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEquivalentTo(pages.Where(p => !selectedPages.Contains(p)));
    }

    [Test]
    public void PastCommand_ShouldSplitClipboardAndAddPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1", 0), new("page2", 0)}.ToObservableCollection();
        _systemService.GetClipboardTextAsync()
            .Returns($"page3{Environment.NewLine}fewfew{Environment.NewLine}ofiewf203{Environment.NewLine} foiwej   ");

        // act
        _sut.PasteCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel>
            {new("page1", 0), new("page2", 0), new("page3", 0), new("fewfew", 0), new("ofiewf203", 0), new("foiwej", 0)}.ToObservableCollection());
    }

    [Test]
    public void RemoveSelectedCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(10);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages.Clear();

        // act
        _sut.RemoveSelectedCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveSelectedCommand_ShouldRemoveSelectedPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(10);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(4);
        IEnumerable<WikiPageModel>? notSelectedPages = pages.Where(p => !selectedPages.Contains(p));
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = selectedPages.ToObservableCollection();

        // act
        _sut.RemoveSelectedCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(notSelectedPages);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveAllCommand_ShouldRemoveAllPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(10);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(4);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = selectedPages.ToObservableCollection();

        // act
        _sut.RemoveAllCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEmpty();
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveDuplicate_ShouldRemoveDuplicatePages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.WikiPageModelFaker.Generate(10);
        pages = pages.Concat(pages.RandomSubset(4)).ToList();
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = pages.RandomSubset(2).ToObservableCollection();

        // act
        _sut.RemoveDuplicateCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages.Distinct());
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveNonMainSpaceCommand_ShouldRemoveNonMainSpacePages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel>
            {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3", 0), new("Page5", 0)}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1", 0), new("page2", 0)}.ToObservableCollection();

        // act
        _sut.RemoveNonMainSpaceCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("Page3", 0), new("Page5", 0)});
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void MoveToTopCommand_ShouldMoveSelectedPagesToTheTop()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel>
            {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3", 0), new("Page5", 0)}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("category:page2", 14), new("Page3", 0)}.ToObservableCollection();

        // act
        _sut.MoveToTopCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("category:page2", 14), new("Page3", 0), new("template:page1", 9), new("user:Page2", 4), new("Page5", 0)},
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void MoveToBottomCommand_ShouldMoveSelectedPagesToTheBottom()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel>
            {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3", 0), new("Page5", 0)}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("category:page2", 14), new("Page3", 0)}.ToObservableCollection();

        // act
        _sut.MoveToBottomCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("template:page1", 9), new("user:Page2", 4), new("Page5", 0), new("category:page2", 14), new("Page3", 0)},
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SortAlphabeticallyCommand_ShouldSortAlphabetically()
    {
        // arrange
        _sut.Pages = Fakers.WikiPageModelFaker.Generate(10).ToObservableCollection();

        // act
        _sut.SortAlphabeticallyCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                _sut.Pages.OrderBy(x => x.Title),
                options => options.WithStrictOrdering());
    }

    [Test]
    public void SortReverseAlphabeticallyCommand_ShouldSortReverseAlphabetically()
    {
        // arrange
        _sut.Pages = Fakers.WikiPageModelFaker.Generate(10).ToObservableCollection();

        // act
        _sut.SortReverseAlphabeticallyCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                _sut.Pages.OrderByDescending(x => x.Title),
                options => options.WithStrictOrdering());
    }
}