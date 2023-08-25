using System.Collections.ObjectModel;
using CrossWikiEditor.ListProviders;
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
        _sut = new MakeListViewModel(_dialogService, listProviders);
    }

    [Test]
    public void AddNewPageCommand_ShouldDoNothing_WhenNewPageTitleIsEmpty([Values("", "  ", null)]string newPageTitle)
    {
        // arrange
        _sut.NewPageTitle = newPageTitle;
        var originalPages = new List<string> {"Page1", "Page2", "Page3"};
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
        var originalPages = new List<string> {"Page1", "Page2", "Page3"};
        _sut.Pages = originalPages.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo("Page1", "Page2", "Page3", "new page");
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
    public void AddNewPageCommand_ShouldNotAddNewPage_WhenItAlreadyIsInTheList()
    {
        // arrange
        _sut.NewPageTitle = "new page title ";
        _sut.Pages = new[] {"new page title"}.ToObservableCollection();

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo("new page title");
    }

    [Test]
    public void AddNewPageCommand_ShouldTrimPage_WhenNewPageTitleContainsWhitespaces()
    {
        // arrange
        _sut.NewPageTitle = "    new page title   ";

        // act
        _sut.AddNewPageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo("new page title");
    }

    [Test]
    public void RemoveCommand_ShouldDoNothing_WhenThereIsNoSelectedPage()
    {
        // arrange
        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new ObservableCollection<string>();

        // act
        _sut.RemoveCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemoveCommand_ShouldRemoveSelectedPages_WhenSelectedPageIsInPages()
    {
        // arrange
        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new []{randomPages[3], randomPages[6], randomPages[1]}.ToObservableCollection();
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

        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPages = new List<string>{"fwe fwe", "ifowe ewiofnwekj fwe"}.ToObservableCollection();

        // act
        _sut.RemoveCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }
    
    
    [Test]
    public void RemoveCommand_ShouldClearSelectedPage()
    {
        // arrange
        _sut.SelectedPages = new List<string>{"", " ", "fioew "}.ToObservableCollection();

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
        
        List<string> existingPages = Fakers.WordsFaker(10);
        _sut.Pages = existingPages.ToObservableCollection();
        
        listProvider.CanMake.Returns(true);
        
        List<string> newPages = Fakers.WordsFaker(10);
        listProvider.MakeList().Returns(Result<List<string>>.Success(newPages));

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
        listProvider.MakeList().Returns(Result<List<string>>.Failure("error message"));
        List<string> existingPages = Fakers.WordsFaker(10);
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
}