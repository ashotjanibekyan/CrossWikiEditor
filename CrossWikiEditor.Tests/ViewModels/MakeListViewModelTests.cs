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
        _sut = new MakeListViewModel(_dialogService, _wikiClientCache, _systemService, _userPreferencesService, listProviders);
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
}