using System.Text;

namespace CrossWikiEditor.Tests.ViewModels;

public sealed class FilterViewModelTests : BaseTest
{
    private FilterViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new FilterViewModel(
        [
            new(0, ""),
            new(2, "Մասնակից"),
            new(4, "Վիքիպեդիա"),
            new(6, "Պատկեր"),
            new(8, "MediaWiki"),
            new(10, "Կաղապար"),
            new(12, "Օգնություն"),
            new(14, "Կատեգորիա"),
        ],
        [
            new(1, "Քննարկում"),
            new(3, "Մասնակցի քննարկում"),
            new(5, "Վիքիպեդիայի քննարկում"),
            new(7, "Պատկերի քննարկում"),
            new(9, "MediaWiki քննարկում"),
            new(11, "Կաղապարի քննարկում"),
            new(13, "Օգնության քննարկում"),
            new(15, "Կատեգորիայի քննարկում"),
        ], new TextFileListProvider(_fileDialogService, _systemService, _settingsService, _wikiClientCache));
        _settingsService.CurrentApiUrl.Returns("https://hy.wikipedia.org/w/api.php?");
    }

    [Test]
    public void IsAllTalkChecked_ShouldCheckAllTalkPages_WhenIsAllTalkCheckedIsSetTrue()
    {
        // arrange
        _sut.IsAllTalkChecked = false;

        // act
        _sut.IsAllTalkChecked = true;

        // assert
        _sut.TalkNamespaces.All(x => x.IsChecked).Should().BeTrue();
    }

    [Test]
    public void IsAllTalkChecked_ShouldUncheckAllTalkPages_WhenIsAllTalkCheckedIsSetFalse()
    {
        // arrange
        _sut.IsAllTalkChecked = true;

        // act
        _sut.IsAllTalkChecked = false;

        // assert
        _sut.TalkNamespaces.All(x => !x.IsChecked).Should().BeTrue();
    }

    [Test]
    public void IsAllSubjectChecked_ShouldCheckAllSubjectPages_WhenIsAllSubjectCheckedIsSetTrue()
    {
        // arrange
        _sut.IsAllSubjectChecked = false;

        // act
        _sut.IsAllSubjectChecked = true;

        // assert
        _sut.SubjectNamespaces.All(x => x.IsChecked).Should().BeTrue();
    }

    [Test]
    public void IsAllSubjectChecked_ShouldUncheckAllSubjectPages_WhenIsAllSubjectCheckedIsSetFalse()
    {
        // arrange
        _sut.IsAllSubjectChecked = true;

        // act
        _sut.IsAllSubjectChecked = false;

        // assert
        _sut.SubjectNamespaces.All(x => !x.IsChecked).Should().BeTrue();
    }

    [Test]
    public void CloseCommand_ShouldCloseWithNull()
    {
        // arrange
        IDialog dialog = Substitute.For<IDialog>();

        // act
        _sut.CloseCommand.Execute(dialog);

        // assert
        dialog.Received(1).Close(null);
    }

    [Test]
    public void OpenFileCommand_ShouldSetPages_WhenListProviderMakesSuccessful()
    {
        // arrange
        const string text = """
                      #[[title1]]


                      # [[Category:title (f e )2|few (fewcas)]]


                      #     [[title3]]
                      #     [[titl e3|display]]
                      """;
        _fileDialogService
            .OpenFilePickerAsync("Select text files to extract pages", true)
            .Returns(["some/path/text.txt"]);
        _systemService
            .ReadAllTextAsync("some/path/text.txt", Encoding.Default)
            .Returns(text);

        // act
        _sut.OpenFileCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEquivalentTo(new List<WikiPageModel>
        {
            new("title1", _settingsService.CurrentApiUrl, _wikiClientCache),
            new("Category:title (f e )2", _settingsService.CurrentApiUrl, _wikiClientCache),
            new("title3", _settingsService.CurrentApiUrl, _wikiClientCache),
            new("titl e3", _settingsService.CurrentApiUrl, _wikiClientCache)
        });
    }

    [Test]
    public void SaveCommand_ShouldCloseWithViewModelValues()
    {
        // arrange
        _sut.SubjectNamespaces[0].IsChecked = true;
        _sut.SubjectNamespaces[4].IsChecked = true;
        _sut.TalkNamespaces[1].IsChecked = true;
        _sut.TalkNamespaces[6].IsChecked = true;
        _sut.RemoveDuplicates = true;
        _sut.RemoveTitlesContaining = "few";
        _sut.KeepTitlesContaining = "as";
        _sut.UseRegex = true;
        _sut.SelectedSetOperations = SetOperations.Intersection;
        _sut.Pages = Fakers.GetWikiPageModelFaker(_settingsService.CurrentApiUrl, _wikiClientCache)
                           .Generate(10).ToObservableCollection();
        IDialog dialog = Substitute.For<IDialog>();
        var namespacesToKeep = _sut.SubjectNamespaces.Where(n => n.IsChecked).Select(n => n.Id).ToList();
        namespacesToKeep.AddRange(_sut.TalkNamespaces.Where(n => n.IsChecked).Select(n => n.Id));

        // act
        _sut.SaveCommand.Execute(dialog);

        // assert
        dialog.Received(1).Close(Arg.Is<FilterOptions>(filter => filter.RemoveDuplicates == _sut.RemoveDuplicates &&
                                                                 filter.RemoveTitlesContaining == _sut.RemoveTitlesContaining &&
                                                                 filter.KeepTitlesContaining == _sut.KeepTitlesContaining &&
                                                                 filter.UseRegex == _sut.UseRegex &&
                                                                 _sut.SortAlphabetically == filter.SortAlphabetically &&
                                                                 filter.SetOperation == _sut.SelectedSetOperations &&
                                                                 filter.FilterPages.SequenceEqual(_sut.Pages) &&
                                                                 filter.NamespacesToKeep.SequenceEqual(namespacesToKeep)));
    }

    [Test]
    public void ClearCommand_ShouldRemoveAllPages()
    {
        // arrange
        _sut.Pages = Fakers.GetWikiPageModelFaker(_settingsService.CurrentApiUrl, _wikiClientCache)
            .Generate(10).ToObservableCollection();

        // act
        _sut.ClearCommand.Execute(null);

        // assert
        _sut.Pages.Should().BeEmpty();
    }
}