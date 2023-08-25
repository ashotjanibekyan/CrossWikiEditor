using Bogus;
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
        _sut = new MakeListViewModel();
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
    public void RemovePageCommand_ShouldDoNothing_WhenThereIsNoSelectedPage([Values("", null)] string selectedPage)
    {
        // arrange
        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPage = selectedPage;

        // act
        _sut.RemovePageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemovePageCommand_ShouldRemoveSelectedPage_WhenSelectedPageIsInPages()
    {
        // arrange
        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPage = randomPages[3];
        randomPages.RemoveAt(3);

        // act
        _sut.RemovePageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }

    [Test]
    public void RemovePageCommand_ShouldDoNothing_WhenSelectedPageIsNotInPages()
    {

        List<string> randomPages = Fakers.WordsFaker(10);
        _sut.Pages = randomPages.ToObservableCollection();
        _sut.SelectedPage = "ugiui uihui ho";

        // act
        _sut.RemovePageCommand.Execute().Subscribe();

        // assert
        _sut.Pages.Should().BeEquivalentTo(randomPages);
    }
    
    
    [Test]
    public void RemovePageCommand_ShouldClearSelectedPage([Values("", null, " ", "new title")] string selectedPage)
    {
        // arrange
        _sut.SelectedPage = selectedPage;

        // act
        _sut.RemovePageCommand.Execute().Subscribe();

        // assert
        _sut.SelectedPage.Should().BeEmpty();
    }
}