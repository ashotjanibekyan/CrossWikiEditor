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
        _sut = new MakeListViewModel(_dialogService, _wikiClientCache, _pageService, _systemService, _fileDialogService, _userPreferencesService, listProviders);
    }

    [Test]
    public void AddNewPageCommand_ShouldDoNothing_WhenNewPageTitleIsEmpty([Values("", "  ", null)]string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;
        var originalPages = new List<WikiPageModel> {new("Page1"), new("Page2"), new("Page3")};
        _sut.Pages = originalPages.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(originalPages);
    }

    [Test]
    public void AddNewPageCommand_ShouldAddPageToTheList_WhenNewPageTitleIsNotEmpty()
    {
        // arrange
        _sut.NewPageTitle = "new page";
        var originalPages = new List<WikiPageModel> {new("Page1"), new("Page2"), new("Page3")};
        _sut.Pages = originalPages.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Select(x => x.Title).Should().BeEquivalentTo("Page1", "Page2", "Page3", "new page");
    }

    [Test]
    public void AddNewPageCommand_ShouldClearNewPageTitle([Values("", null, " ", "new title")] string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.NewPageTitle.Should().BeEmpty();
    }

    [Test]
    public void AddNewPageCommand_ShouldTrimPage_WhenNewPageTitleContainsWhitespaces()
    {
        // arrange
        _sut.NewPageTitle = "    new page title   ";

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel>{new("new page title")});
    }

    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        var randomPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.RemoveCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemoveCommand_ShouldRemoveSelectedPages_WhenSelectedPageIsInPages()
    {
        // arrange
        var randomPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new []{_sut.Pages[3], _sut.Pages[6], _sut.Pages[1]}.ToObservableCollection();
        randomPages.RemoveAt(1);
        randomPages.RemoveAt(2);
        randomPages.RemoveAt(4);

        // act
        _sut.RemoveCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenSelectedPageIsNotInPages()
    {

        var randomPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel>{new("fwe fwe"), new("ifowe ewiofnwekj fwe")}.ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }
    
    
    [Test]
    public void RemoveCommand_ShouldClearSelectedPage()
    {
        // arrange
        _sut.SelectedPages = new List<WikiPageModel>{new(""), new(" "), new("fioew ")}.ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute().Subscribe();

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
        _sut.MakeListCommand.Execute().Subscribe();

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
        
        var existingPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        _sut.Pages = existingPages.ToObservableCollection();
        
        listProvider.CanMake.Returns(true);
        
        var newPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        listProvider.MakeList().Returns(Result<List<WikiPageModel>>.Success(newPages));

        // act
        _sut.MakeListCommand.Execute().Subscribe();

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
        var existingPages = Fakers.WordsFaker(10).Select(x => new WikiPageModel(x)).ToList();
        _sut.Pages = existingPages.ToObservableCollection();

        // act
        _sut.MakeListCommand.Execute().Subscribe();

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
        _sut.MakeListCommand.Execute().Subscribe();

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
        _sut.MakeListCommand.Execute().Subscribe();

        // assert
        listProvider.Received(0).GetAdditionalParams();
    }

    [Test]
    public void OpenInBrowserCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.OpenInBrowserCommand.Execute().Subscribe();

        // assert
        _systemService.Received(0).OpenLinkInBrowser(Arg.Any<string>());
    }

    [Test]
    public void OpenInBrowserCommand_ShouldOpenAllSelectedPages_WhenThereAreSelectedPages()
    {
        // arrange
        _sut.SelectedPages = Fakers.WordsFaker(4).Select(x => new WikiPageModel(x)).ToObservableCollection();
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _systemService.OpenLinkInBrowser(Arg.Any<string>()).Returns(Result.Success());

        // act
        _sut.OpenInBrowserCommand.Execute().Subscribe();

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
        _sut.OpenHistoryInBrowserCommand.Execute().Subscribe();

        // assert
        _systemService.Received(0).OpenLinkInBrowser(Arg.Any<string>());
    }

    [Test]
    public void OpenHistoryInBrowserCommand_ShouldOpenAllSelectedPages_WhenThereAreSelectedPages()
    {
        // arrange
        _sut.SelectedPages = Fakers.WordsFaker(4).Select(x => new WikiPageModel(x)).ToObservableCollection();
        _userPreferencesService.GetCurrentPref().Returns(new UserPrefs
        {
            LanguageCode = "hy",
            Project = ProjectEnum.Wikipedia
        });
        _systemService.OpenLinkInBrowser(Arg.Any<string>()).Returns(Result.Success());

        // act
        _sut.OpenHistoryInBrowserCommand.Execute().Subscribe();

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
        _sut.CutCommand.Execute().Subscribe();

        // assert
        _systemService.DidNotReceive().SetClipboardTextAsync(Arg.Any<string>());
    }

    [Test]
    public void CutCommand_ShouldCutSelectedPageTitlesAndRemoveThem()
    {
        // arrange
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2")}.ToObservableCollection();
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();

        // act
        _sut.CutCommand.Execute().Subscribe();

        // assert
        _systemService.Received(1).SetClipboardTextAsync("page1\npage2\nPage2");
        _sut.SelectedPages.Should().BeEmpty();
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("Page3"), new("Page5")}.ToObservableCollection());
    }
    
    

    [Test]
    public void CopyCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.CopyCommand.Execute().Subscribe();

        // assert
        _clipboard.DidNotReceive().SetTextAsync(Arg.Any<string>());
    }

    [Test]
    public void CopyCommand_ShouldCutSelectedPageTitlesAndRemoveThem()
    {
        // arrange
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2")}.ToObservableCollection();
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();

        // act
        _sut.CopyCommand.Execute().Subscribe();

        // assert
        _systemService.Received(1).SetClipboardTextAsync("page1\npage2\nPage2");
        _sut.SelectedPages.Should().BeEquivalentTo(new List<WikiPageModel> {new("page1"), new("page2"), new("Page2")});
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")});
    }

    [Test]
    public void SelectAllCommand_ShouldSelectAllPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectAllCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.Pages.Should().BeEquivalentTo(_sut.SelectedPages);
    }

    [Test]
    public void SelectNoneCommand_ShouldUnSelectAllPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectNoneCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SelectInverseCommand_ShouldSelectAll_WhenNonIsSelected()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();
        
        // act
        _sut.SelectInverseCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.Pages.Should().BeEquivalentTo(_sut.SelectedPages);
    }

    [Test]
    public void SelectInverseCommand_ShouldUnselectAll_WhenEverythingIsSelected()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        
        // act
        _sut.SelectInverseCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SelectInverseCommand_ShouldInverseSelection()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();
        
        // act
        _sut.SelectInverseCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEquivalentTo(new List<WikiPageModel> {new("Page2"), new("Page3"), new("Page5")});
    }

    [Test]
    public void PastCommand_ShouldSplitClipboardAndAddPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();
        _systemService.GetClipboardTextAsync().Returns($"page3{Environment.NewLine}fewfew{Environment.NewLine}ofiewf203{Environment.NewLine} foiwej   ");

        // act
        _sut.PasteCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel>
            {new("page1"), new("page2"), new("page3"), new("fewfew"), new("ofiewf203"), new("foiwej")}.ToObservableCollection());
    }

    [Test]
    public void RemoveSelectedCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages.Clear();

        // act
        _sut.RemoveSelectedCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")});
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveSelectedCommand_ShouldRemoveSelectedPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();

        // act
        _sut.RemoveSelectedCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("Page2"), new("Page3"), new("Page5")});
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveAllCommand_ShouldRemoveAllPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();
        
        // act
        _sut.RemoveAllCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEmpty();
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveDuplicate_ShouldRemoveDuplicatePages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("page1"), new("page1"), new("page1"), new("page2"), new("Page2"), new("Page2"), new("Page2"), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();
        
        // act
        _sut.RemoveDuplicateCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("page1"), new("page2"), new("Page2"), new("Page3"), new("Page5")});
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void RemoveNonMainSpaceCommand_ShouldRemoveNonMainSpacePages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("page1"), new("page2")}.ToObservableCollection();
        
        // act
        _sut.RemoveNonMainSpaceCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> {new("Page3"), new("Page5")});
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void MoveToTopCommand_ShouldMoveSelectedPagesToTheTop()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("category:page2", 14), new("Page3")}.ToObservableCollection();
        
        // act
        _sut.MoveToTopCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("category:page2", 14), new("Page3"), new("template:page1", 9), new("user:Page2", 4), new("Page5")},
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }
    
    [Test]
    public void MoveToBottomCommand_ShouldMoveSelectedPagesToTheBottom()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("template:page1", 9), new("category:page2", 14), new("user:Page2", 4), new("Page3"), new("Page5")}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> {new("category:page2", 14), new("Page3")}.ToObservableCollection();
        
        // act
        _sut.MoveToBottomCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("template:page1", 9), new("user:Page2", 4), new("Page5"),new("category:page2", 14), new("Page3")},
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }

    [Test]
    public void SortAlphabeticallyCommand_ShouldSortAlphabetically()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("abc"), new("aev"), new("grw"), new("aaa"), new("23fwe")}.ToObservableCollection();

        // act
        _sut.SortAlphabeticallyCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("23fwe"), new("aaa"), new("abc"),new("aev"), new("grw")},
                options => options.WithStrictOrdering());
    }

    [Test]
    public void SortReverseAlphabeticallyCommand_ShouldSortReverseAlphabetically()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> {new("abc"), new("aev"), new("grw"), new("aaa"), new("23fwe")}.ToObservableCollection();

        // act
        _sut.SortReverseAlphabeticallyCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> {new("grw"), new("aev"), new("abc"), new("aaa"), new("23fwe")},
                options => options.WithStrictOrdering());
    }
}