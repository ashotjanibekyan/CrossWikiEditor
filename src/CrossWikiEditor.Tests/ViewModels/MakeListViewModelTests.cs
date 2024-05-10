using System.Collections.ObjectModel;
using System.Text;
using System.Web;

using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Tests.ViewModels;

public sealed class MakeListViewModelTests : BaseTest
{
    private MakeListViewModel _sut;
    private const string ApiRoot = "https://hy.wikipedia.org/w/api.php?";
    private readonly WikiClient _wikiClient = new();

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        List<IListProvider> listProviders = new()
        {
            Substitute.For<IListProvider>(),
            Substitute.For<IListProvider>()
        };
        _sut = new MakeListViewModel(_messenger, _logger, _dialogService, _wikiClientCache, _pageService, _systemService, _viewModelFactory,
            _fileDialogService, _userPreferencesService, listProviders);
        _userPreferencesService.GetCurrentSettings().Returns(new UserSettings()
        {
            UserWiki = new("hy", ProjectEnum.Wikipedia)
        });
        _userPreferencesService.CurrentApiUrl.Returns(ApiRoot);
        _wikiClientCache.GetWikiSite(Arg.Any<string>()).Returns(new WikiSite(_wikiClient, ApiRoot));
    }

    #region Messenger
    [Test]
    public void Messenger_ShouldRemovePage_WhenPageUpdatedMessageReceived()
    {
        // arrange
        var pages = new List<WikiPageModel>
        {
            new("Page1", ApiRoot, _wikiClientCache),
            new("Page2", ApiRoot, _wikiClientCache),
            new("Page3", ApiRoot, _wikiClientCache),
            new("Page4", ApiRoot, _wikiClientCache),
        };
        List<IListProvider> listProviders = new()
        {
            Substitute.For<IListProvider>(),
            Substitute.For<IListProvider>()
        };
        var messenger = new MessengerWrapper(WeakReferenceMessenger.Default);
        _sut = new MakeListViewModel(messenger, _logger, _dialogService, _wikiClientCache, _pageService, _systemService, _viewModelFactory,
            _fileDialogService, _userPreferencesService, listProviders)
        {
            Pages = pages.ToObservableCollection()
        };
        // act
        messenger.Send(new PageUpdatedMessage(new WikiPageModel("Page2", ApiRoot, _wikiClientCache)));

        // assert
        _sut.Pages.Count.Should().Be(3);
        _sut.Pages[0].Title.Should().Be("Page1");
        _sut.Pages[1].Title.Should().Be("Page3");
        _sut.Pages[2].Title.Should().Be("Page4");
    }
    #endregion

    #region AddNewPageCommand
    [Test]
    public void AddNewPageCommand_ShouldDoNothing_WhenNewPageTitleIsEmpty([Values("", "  ")] string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;
        var originalPages = new List<WikiPageModel> { new("Page1", ApiRoot, _wikiClientCache), new("Page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache) };
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
        List<WikiPageModel>? originalPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(3);
        _sut.Pages = originalPages.ToObservableCollection();
        _wikiClientCache.GetWikiPageModel(ApiRoot, "new page")
            .Returns(args => Result<WikiPageModel>.Success(new WikiPageModel("new page", ApiRoot, _wikiClientCache)));

        // act
        _sut.AddNewPageCommand.Execute(null);
        
        // assert
        _sut.Pages.Should().BeEquivalentTo(originalPages.Concat([new WikiPageModel("new page", ApiRoot, _wikiClientCache)]));
    }

    [Test]
    public void AddNewPageCommand_ShouldClearNewPageTitle([Values("", " ", "new title")] string newPageTitle)
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
        _wikiClientCache.GetWikiPageModel(ApiRoot, Arg.Any<string>())
            .Returns(args => Result<WikiPageModel>.Success(new WikiPageModel("new page title", ApiRoot, _wikiClientCache)));
        
        // act
        _sut.AddNewPageCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> { new("new page title", ApiRoot, _wikiClientCache) });
    }
    #endregion

    #region RemoveCommand
    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        List<WikiPageModel>? randomPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
        List<WikiPageModel>? randomPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new[] { _sut.Pages[3], _sut.Pages[6], _sut.Pages[1] }.ToObservableCollection<WikiPageModel>();
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
        List<WikiPageModel>? randomPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(2)
            .Select(x => new WikiPageModel(x.Title + Guid.NewGuid(), ApiRoot, _wikiClientCache))
            .ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }


    [Test]
    public void RemoveCommand_ShouldClearSelectedPage()
    {
        // arrange
        _sut.SelectedPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(3).ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute(null);

        // assert
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region MakeListCommand
    [Test]
    public void MakeListCommand_ShouldGetLimit_WhenListProviderIsLimited()
    {
        // arrange
        ILimitedListProvider listProvider = Substitute.For<ILimitedListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.CanMake.Returns(true);
        listProvider.GetLimit().Returns(42);
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(42);
        listProvider.MakeList(42).Returns(Result<List<WikiPageModel>>.Success(pages));
        
        // act
        _sut.MakeListCommand.Execute(42);

        // assert
        listProvider.Received(1).GetLimit();
        listProvider.Received(1).MakeList(42);
        _sut.Pages.Should().BeEquivalentTo(pages);
    }
    
    [Test]
    public void MakeListCommand_ShouldDoNothing_WhenSelectedProviderCanNotMake()
    {
        // arrange
        IUnlimitedListProvider listProvider = Substitute.For<IUnlimitedListProvider>();
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
        IUnlimitedListProvider listProvider = Substitute.For<IUnlimitedListProvider>();
        _sut.SelectedListProvider = listProvider;

        List<WikiPageModel>? existingPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = existingPages.ToObservableCollection();

        listProvider.CanMake.Returns(true);

        List<WikiPageModel>? newPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
        IUnlimitedListProvider listProvider = Substitute.For<IUnlimitedListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.CanMake.Returns(true);
        listProvider.MakeList().Returns(Result<List<WikiPageModel>>.Failure("error message"));
        List<WikiPageModel>? existingPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
        INeedAdditionalParamsListProvider listProvider = Substitute.For<INeedAdditionalParamsListProvider>();
        _sut.SelectedListProvider = listProvider;

        // act
        _sut.MakeListCommand.Execute(null);

        // assert
        listProvider.Received(1).GetAdditionalParams();
    }
    #endregion

    #region OpenInBrowserCommand
    [Test]
    public void OpenInBrowserCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        _sut.SelectedPages = [];

        // act
        _sut.OpenInBrowserCommand.Execute(null);

        // assert
        _systemService.Received(0).OpenLinkInBrowser(Arg.Any<string>());
    }

    [Test]
    public void OpenInBrowserCommand_ShouldOpenAllSelectedPages_WhenThereAreSelectedPages()
    {
        // arrange
        _sut.SelectedPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(4).ToObservableCollection();
        _userPreferencesService.GetCurrentSettings().Returns(new UserSettings
        {
            UserWiki = new("hy", ProjectEnum.Wikipedia)
        });
        _systemService.OpenLinkInBrowser(Arg.Any<string>()).Returns(Result.Success());

        // act
        _sut.OpenInBrowserCommand.Execute(null);

        // assert
        Received.InOrder(() =>
        {
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={HttpUtility.UrlEncode(_sut.SelectedPages[0].Title)}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={HttpUtility.UrlEncode(_sut.SelectedPages[1].Title)}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={HttpUtility.UrlEncode(_sut.SelectedPages[2].Title)}");
            _systemService.OpenLinkInBrowser($"https://hy.wikipedia.org/w/index.php?title={HttpUtility.UrlEncode(_sut.SelectedPages[3].Title)}");
        });
    }
    #endregion

    #region OpenHistoryInBrowserCommand
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
        _sut.SelectedPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(4).ToObservableCollection();
        _userPreferencesService.GetCurrentSettings().Returns(new UserSettings
        {
            UserWiki = new("hy", ProjectEnum.Wikipedia)
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
    #endregion

    #region CutCommand
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
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> { _sut.Pages[2], _sut.Pages[4], _sut.Pages[1] }.ToObservableCollection();

        // act
        _sut.CutCommand.Execute(null);

        // assert
        _systemService.Received(1)
            .SetClipboardTextAsync($"{pages[2].Title}{Environment.NewLine}{pages[4].Title}{Environment.NewLine}{pages[1].Title}");
        _sut.SelectedPages.Should().BeEmpty();
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel> { pages[0], pages[3] }.ToObservableCollection());
    }
    #endregion

    #region CopyCommand
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
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
    #endregion

    #region SelectAllCommand
    [Test]
    public void SelectAllCommand_ShouldSelectAllPages()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5).ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectAllCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.Pages.Should().BeEquivalentTo(_sut.SelectedPages);
    }
    #endregion

    #region SelectNoneCommand
    [Test]
    public void SelectNoneCommand_ShouldUnSelectAllPages()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5).ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<WikiPageModel>();

        // act
        _sut.SelectNoneCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region SelectInverseCommand
    [Test]
    public void SelectInverseCommand_ShouldSelectAll_WhenNonIsSelected()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5).ToObservableCollection();
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
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
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
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(2);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = selectedPages.ToObservableCollection();

        // act
        _sut.SelectInverseCommand.Execute(null);

        // assert
        _sut.Pages.Should().HaveCount(5);
        _sut.SelectedPages.Should().BeEquivalentTo(pages.Where(p => !selectedPages.Contains(p)));
    }
    #endregion

    #region PastCommand
    [Test]
    public void PastCommand_ShouldSplitClipboardAndAddPages()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel> { new("page1", ApiRoot, _wikiClientCache), new("page2", ApiRoot, _wikiClientCache) }.ToObservableCollection();
        _systemService.GetClipboardTextAsync()
            .Returns($"page3{Environment.NewLine}fewfew{Environment.NewLine}ofiewf203{Environment.NewLine} foiwej   ");
        _wikiClientCache.GetWikiPageModel(ApiRoot, Arg.Any<string>())
            .Returns(args => Result<WikiPageModel>.Success(new WikiPageModel(((string) args[1]).Trim(), ApiRoot, _wikiClientCache)));
        
        // act
        _sut.PasteCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel>
        {
            new("page1", ApiRoot, _wikiClientCache), new("page2", ApiRoot, _wikiClientCache), new("page3", ApiRoot, _wikiClientCache),
            new("fewfew", ApiRoot, _wikiClientCache), new("ofiewf203", ApiRoot, _wikiClientCache), new("foiwej", ApiRoot, _wikiClientCache)
        }.ToObservableCollection());
    }
    #endregion

    #region RemoveSelectedCommand
    [Test]
    public void RemoveSelectedCommand_ShouldDoNothing_WhenSelectedPagesIsEmpty()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
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
    #endregion

    #region RemoveAllCommand
    [Test]
    public void RemoveAllCommand_ShouldRemoveAllPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        List<WikiPageModel>? selectedPages = pages.RandomSubset(4);
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = selectedPages.ToObservableCollection();

        // act
        _sut.RemoveAllCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEmpty();
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region RemoveDuplicate
    [Test]
    public void RemoveDuplicate_ShouldRemoveDuplicatePages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        pages = pages.Concat(pages.RandomSubset(4)).ToList();
        _sut.Pages = pages.ToObservableCollection();
        _sut.SelectedPages = pages.RandomSubset(2).ToObservableCollection();

        // act
        _sut.RemoveDuplicateCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages.Distinct());
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region RemoveNonMainSpaceCommand
    [Test]
    public void RemoveNonMainSpaceCommand_ShouldRemoveNonMainSpacePages()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5).ToObservableCollection();
        _sut.SelectedPages = _sut.Pages.ToList().RandomSubset<WikiPageModel>(3).ToObservableCollection();
        _sut.Pages[0].NamespaceId = 0;
        _sut.Pages[1].NamespaceId = 2;
        _sut.Pages[2].NamespaceId = 0;
        _sut.Pages[3].NamespaceId = 12;
        _sut.Pages[4].NamespaceId = 13;
        var nonSelected = new List<WikiPageModel> {_sut.Pages[0], _sut.Pages[2]};

        // act
        _sut.RemoveNonMainSpaceCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(nonSelected);
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region MoveToTopCommand
    [Test]
    public void MoveToTopCommand_ShouldMoveSelectedPagesToTheTop()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel>
            {new("template:page1", ApiRoot, _wikiClientCache), new("category:page2", ApiRoot, _wikiClientCache), new("user:Page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache), new("Page5", ApiRoot, _wikiClientCache)}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> { new("category:page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache) }.ToObservableCollection();

        // act
        _sut.MoveToTopCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> { new("category:page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache), new("template:page1", ApiRoot, _wikiClientCache), new("user:Page2", ApiRoot, _wikiClientCache), new("Page5", ApiRoot, _wikiClientCache) },
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region MoveToBottomCommand
    [Test]
    public void MoveToBottomCommand_ShouldMoveSelectedPagesToTheBottom()
    {
        // arrange
        _sut.Pages = new List<WikiPageModel>
            {new("template:page1", ApiRoot, _wikiClientCache), new("category:page2", ApiRoot, _wikiClientCache), new("user:Page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache), new("Page5", ApiRoot, _wikiClientCache)}.ToObservableCollection();
        _sut.SelectedPages = new List<WikiPageModel> { new("category:page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache) }.ToObservableCollection();

        // act
        _sut.MoveToBottomCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                new List<WikiPageModel> { new("template:page1", ApiRoot, _wikiClientCache), new("user:Page2", ApiRoot, _wikiClientCache), new("Page5", ApiRoot, _wikiClientCache), new("category:page2", ApiRoot, _wikiClientCache), new("Page3", ApiRoot, _wikiClientCache) },
                options => options.WithStrictOrdering());
        _sut.SelectedPages.Should().BeEmpty();
    }
    #endregion

    #region ConvertToTalkPagesCommand
    [Test]
    public void ConvertToTalkPagesCommand_ShouldConvertPagesToTalkPages_WhenPageServiceReturnsSuccess()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        List<WikiPageModel>? talkPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = pages.ToObservableCollection();
        _pageService.ConvertToTalk(Arg.Is<List<WikiPageModel>>(argPages => argPages.SequenceEqual(pages)))
                    .Returns(Result<List<WikiPageModel>>.Success(talkPages));

        // act
        _sut.ConvertToTalkPagesCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(talkPages);
    }
    
    
    [Test]
    public void ConvertToTalkPagesCommand_ShouldDoNothing_WhenPageServiceReturnsFailure()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = pages.ToObservableCollection();
        _pageService.ConvertToTalk(Arg.Is<List<WikiPageModel>>(argPages => argPages.SequenceEqual(pages)))
            .Returns(Result<List<WikiPageModel>>.Failure("can not convert"));

        // act
        _sut.ConvertToTalkPagesCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages);
    }
    #endregion

    #region ConvertFromTalkPagesCommand
    [Test]
    public void ConvertFromTalkPagesCommand_ShouldConvertPagesFromTalkPages_WhenPageServiceReturnsSuccess()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        List<WikiPageModel>? talkPages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = pages.ToObservableCollection();
        _pageService.ConvertToSubject(Arg.Is<List<WikiPageModel>>(argPages => argPages.SequenceEqual(pages)))
            .Returns(Result<List<WikiPageModel>>.Success(talkPages));

        // act
        _sut.ConvertFromTalkPagesCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(talkPages);
    }
    
    
    [Test]
    public void ConvertFromTalkPagesCommand_ShouldDoNothing_WhenPageServiceReturnsFailure()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _sut.Pages = pages.ToObservableCollection();
        _pageService.ConvertToSubject(Arg.Is<List<WikiPageModel>>(argPages => argPages.SequenceEqual(pages)))
            .Returns(Result<List<WikiPageModel>>.Failure("can not convert"));

        // act
        _sut.ConvertFromTalkPagesCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages);
    }
    #endregion

    #region FilterCommand
    [Test]
    public void FilterCommand_ShouldRemoveDuplicates_WhenRemoveDuplicatesIsTrue()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10);
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", "", false, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages.AddRange(pages);
        _sut.Pages.AddRange(pages);

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(pages);
    }

    [Test]
    public void FilterCommand_ShouldRemoveTitlesContaining_WhenRemoveTitlesContainingIsNotEmpty()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "a", "", false, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bbb", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("bewgrbb", ApiRoot, _wikiClientCache),
            new("abbb", ApiRoot, _wikiClientCache),
            new("aaa", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("bbb");
        _sut.Pages[1].Title.Should().Be("bewgrbb");
    }

    [Test]
    public void FilterCommand_ShouldOnlyKeepTitlesContaining_WhenKeepTitlesContainingIsNotEmpty()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", "a", false, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bbb", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("bewgrbb", ApiRoot, _wikiClientCache),
            new("abbb", ApiRoot, _wikiClientCache),
            new("aaa", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(3);
        _sut.Pages[0].Title.Should().Be("babb");
        _sut.Pages[1].Title.Should().Be("abbb");
        _sut.Pages[2].Title.Should().Be("aaa");
    }

    [Test]
    public void FilterCommand_ShouldRemoveTitlesContainingRegex_WhenRemoveTitlesContainingIsNotEmptyAndUseRegex()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), @"\d", "", true, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("be4wgrbb", ApiRoot, _wikiClientCache),
            new("abbb", ApiRoot, _wikiClientCache),
            new("aa2a", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("babb");
        _sut.Pages[1].Title.Should().Be("abbb");
    }

    [Test]
    public void FilterCommand_ShouldOnlyKeepTitlesContainingRegex_WhenKeepTitlesContainingIsNotEmptyAndUseRegex()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", @"\d", true, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("be4wgrbb", ApiRoot, _wikiClientCache),
            new("abbb", ApiRoot, _wikiClientCache),
            new("aa2a", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(3);
        _sut.Pages[0].Title.Should().Be("bb2b");
        _sut.Pages[1].Title.Should().Be("be4wgrbb");
        _sut.Pages[2].Title.Should().Be("aa2a");
    }

    [Test]
    public async Task FilterCommand_ShouldKeepOnlyPagesInGivenNamespaces_WhenNamespacesToKeepIsNotEmpty()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(new []{0, 1}, "", "", true, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("Մասնակից:babb", ApiRoot, _wikiClientCache),
            new("Քննարկում:be4wgrbb", ApiRoot, _wikiClientCache),
            new("Օգնություն:abbb", ApiRoot, _wikiClientCache),
            new("Վիքիպեդիա:aa2a", ApiRoot, _wikiClientCache),
        });
        await Task.WhenAll(_sut.Pages.Select(p => p.InitAsync));

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("bb2b");
        _sut.Pages[1].Title.Should().Be("Քննարկում:be4wgrbb");
    }

    [Test]
    public void FilterCommand_ShouldKeepIntersection_WhenSetOperationIsIntersectionAndPagesIsNotEmpty()
    {
        // arrange
        var pages = new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("Մասնակից:babb", ApiRoot, _wikiClientCache),
            new("Քննարկում:be4wgrbb", ApiRoot, _wikiClientCache),
            new("Օգնություն:abbb", ApiRoot, _wikiClientCache),
            new("Վիքիպեդիա:aa2a", ApiRoot, _wikiClientCache),
        };
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", "", true, false, true, SetOperations.Intersection, new List<WikiPageModel>
            {
                pages[1],
                pages[3],
            }));
        _sut.Pages = new ObservableCollection<WikiPageModel>(pages);

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("Մասնակից:babb");
        _sut.Pages[1].Title.Should().Be("Օգնություն:abbb");
    }

    [Test]
    public void FilterCommand_ShouldKeepSymmetricDifference_WhenSetOperationIsSymmetricDifferenceAndPagesIsNotEmpty()
    {
        // arrange
        var ogPages = new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("Մասնակից:babb", ApiRoot, _wikiClientCache),
            new("Քննարկում:be4wgrbb", ApiRoot, _wikiClientCache),
            new("Օգնություն:abbb", ApiRoot, _wikiClientCache),
            new("Վիքիպեդիա:aa2a", ApiRoot, _wikiClientCache),
        };
        var filterPages = new List<WikiPageModel>
        {
            new("Վիքիպեդիա:aa2a", ApiRoot, _wikiClientCache),
            new("Կաղապար:aոֆեոa2a", ApiRoot, _wikiClientCache),
            ogPages[0],
            ogPages[3]

        };
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", "", true, false, true, SetOperations.SymmetricDifference, filterPages));
        _sut.Pages = new ObservableCollection<WikiPageModel>(ogPages);

        // act
        _sut.FilterCommand.Execute(null);
        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("Մասնակից:babb");
        _sut.Pages[1].Title.Should().Be("Քննարկում:be4wgrbb");
    }

    [Test]
    public void FilterCommand_ShouldSort_WhenSortAlphabeticallyIsTrue()
    {
        // arrange
        var ogPages = new WikiPageModel[]
        {
            new("bb2b", ApiRoot, _wikiClientCache),
            new("bb3b", ApiRoot, _wikiClientCache),
            new("Մասնակից:babb", ApiRoot, _wikiClientCache),
            new("Քննարկում:be4wgrbb", ApiRoot, _wikiClientCache),
            new("Օգնություն:abbb", ApiRoot, _wikiClientCache),
            new("Վիքիպեդիա:aa2a", ApiRoot, _wikiClientCache),
        };
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "", "", true, true, true, SetOperations.SymmetricDifference, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(ogPages);

        // act
        _sut.FilterCommand.Execute(null);
        // assert
        _sut.Pages.Count.Should().Be(6);
        _sut.Pages[0].Title.Should().Be("bb2b");
        _sut.Pages[1].Title.Should().Be("bb3b");
        _sut.Pages[2].Title.Should().Be("Մասնակից:babb");
        _sut.Pages[3].Title.Should().Be("Վիքիպեդիա:aa2a");
        _sut.Pages[4].Title.Should().Be("Քննարկում:be4wgrbb");
        _sut.Pages[5].Title.Should().Be("Օգնություն:abbb");
    }

    [Test]
    public void FilterCommand_ShouldRemoveTitleContainingAndKeepTitlesContaining_WhenRemoveAndKeepTitlesAreNotEmpty()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), "a", "b", false, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bbb", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("bewgrbb", ApiRoot, _wikiClientCache),
            new("abbb", ApiRoot, _wikiClientCache),
            new("aaa", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("bbb");
        _sut.Pages[1].Title.Should().Be("bewgrbb");
    }
    
    

    [Test]
    public void FilterCommand_ShouldRemoveTitleContainingAndKeepTitlesContainingUseRegex_WhenRemoveAndKeepTitlesAreNotEmpty()
    {
        // arrange
        _dialogService.ShowDialog<FilterOptions>(Arg.Any<FilterViewModel>())
            .Returns(new FilterOptions(Array.Empty<int>(), @"\d", @"(.)\1", true, false, true, SetOperations.Intersection, new List<WikiPageModel>()));
        _sut.Pages = new ObservableCollection<WikiPageModel>(new WikiPageModel[]
        {
            new("bgbggb", ApiRoot, _wikiClientCache),
            new("babb", ApiRoot, _wikiClientCache),
            new("bewg2rbb", ApiRoot, _wikiClientCache),
            new("abgbgb", ApiRoot, _wikiClientCache),
            new("a4aa", ApiRoot, _wikiClientCache),
        });

        // act
        _sut.FilterCommand.Execute(null);

        // assert
        _sut.Pages.Count.Should().Be(2);
        _sut.Pages[0].Title.Should().Be("bgbggb");
        _sut.Pages[1].Title.Should().Be("babb");
    }
    
    #endregion

    #region SaveListCommand
    [Test]
    public void SaveListCommand_ShouldSaveFile_WhenUserPickedALocationAndPagesExist()
    {
        // arrange
        Func<Task<Stream>>? openWriteStream = Substitute.For<Func<Task<Stream>>>();
        Stream stream = Substitute.For<Stream>();
        IListProvider? listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.Title.Returns("listProviderTitle");
        listProvider.Param.Returns("ListProviderParam");
        _fileDialogService.SaveFilePickerAsync("Save pages", defaultExtension: Arg.Any<string?>(), Arg.Any<string?>())
                            .Returns((Substitute.For<Func<Task<Stream>>>(), openWriteStream));
        openWriteStream().Returns(stream);
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(3);
        _sut.Pages = pages.ToObservableCollection();
        
        // act
        _sut.SaveListCommand.Execute(null);

        // assert

        stream.Received(3).WriteAsync(Arg.Any<ReadOnlyMemory<byte>>(), Arg.Any<CancellationToken>());
        Received.InOrder(() =>
        {
            stream.WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(b => Encoding.UTF8.GetString(b.ToArray()) == $"# [[:{pages[0].Title}]]{Environment.NewLine}"));
            stream.WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(b => Encoding.UTF8.GetString(b.ToArray()) == $"# [[:{pages[1].Title}]]{Environment.NewLine}"));
            stream.WriteAsync(Arg.Is<ReadOnlyMemory<byte>>(b => Encoding.UTF8.GetString(b.ToArray()) == $"# [[:{pages[2].Title}]]{Environment.NewLine}"));
        });
        stream.Received(1).Close();
    }
    
    [Test]
    public void SaveListCommand_ShouldDoNothing_WhenUserDidNotPickALocationAndPagesExist()
    {
        // arrange
        IListProvider? listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.Title.Returns("listProviderTitle");
        listProvider.Param.Returns("ListProviderParam");
        _fileDialogService.SaveFilePickerAsync("Save pages", defaultExtension: Arg.Any<string?>(), Arg.Any<string?>())
            .Returns((null, null));
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(3);
        _sut.Pages = pages.ToObservableCollection();
        
        // act
        _sut.SaveListCommand.Execute(null);

        // assert

        _fileDialogService.Received(1).SaveFilePickerAsync("Save pages", defaultExtension: Arg.Any<string?>(), Arg.Any<string?>());
    }
    

    [Test]
    public void SaveListCommand_ShouldDoNothing_WhenUserPagesDoNotExist()
    {
        // arrange
        IListProvider? listProvider = Substitute.For<IListProvider>();
        _sut.SelectedListProvider = listProvider;
        listProvider.Title.Returns("listProviderTitle");
        listProvider.Param.Returns("ListProviderParam");
        _fileDialogService.SaveFilePickerAsync("Save pages", defaultExtension: Arg.Any<string?>(), Arg.Any<string?>())
            .Returns((null, null));
        
        // act
        _sut.SaveListCommand.Execute(null);

        // assert

        _fileDialogService.DidNotReceive().SaveFilePickerAsync("Save pages", defaultExtension: Arg.Any<string?>(), Arg.Any<string?>());
    }
    #endregion

    #region SortAlphabeticallyCommand
    [Test]
    public void SortAlphabeticallyCommand_ShouldSortAlphabetically()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10).ToObservableCollection();

        // act
        _sut.SortAlphabeticallyCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                _sut.Pages.OrderBy(x => x.Title),
                options => options.WithStrictOrdering());
    }
    #endregion

    #region SortReverseAlphabeticallyCommand
    [Test]
    public void SortReverseAlphabeticallyCommand_ShouldSortReverseAlphabetically()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(10).ToObservableCollection();

        // act
        _sut.SortReverseAlphabeticallyCommand.Execute(null);

        // assert
        _sut.Pages.Should()
            .BeEquivalentTo(
                _sut.Pages.OrderByDescending(x => x.Title),
                options => options.WithStrictOrdering());
    }
    #endregion
}