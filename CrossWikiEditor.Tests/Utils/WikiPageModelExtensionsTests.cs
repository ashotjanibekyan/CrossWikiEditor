namespace CrossWikiEditor.Tests.Utils;

public class WikiPageModelExtensionsTests : BaseTest
{
    private const string ApiRoot = "https://hy.wikipedia.org/w/api.php?";

    [SetUp]
    public void SetUp()
    {
        SetUpServices();
    }

    [Test]
    public void ToWikiList_ShouldReturnEmptyString_WhenPagesIsEmpty()
    {
        // arrange
        var pages = new List<WikiPageModel>();

        // act
        string list = pages.ToWikiList(true, 6);

        // assert
        list.Should().Be(string.Empty);
    }

    [Test]
    public void ToWikiList_ShouldMakeNumericList_SectionIsLargerThanPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        string expectedList = $"""
                               == 1 ==
                               # [[{pages[0].Title}]]
                               # [[{pages[1].Title}]]
                               # [[{pages[2].Title}]]
                               # [[{pages[3].Title}]]
                               # [[{pages[4].Title}]]
                               
                               """;
        
        // act
        string list = pages.ToWikiList(true, 6);

        // assert
        list.Should().Be(expectedList);
    }
    
    [Test]
    public void ToWikiList_ShouldMakeNumericList_SectionIsSmallerThanPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        string expectedList = $"""
                               == 1 ==
                               # [[{pages[0].Title}]]
                               # [[{pages[1].Title}]]
                               == 2 ==
                               # [[{pages[2].Title}]]
                               # [[{pages[3].Title}]]
                               == 3 ==
                               # [[{pages[4].Title}]]

                               """;
        
        // act
        string list = pages.ToWikiList(true, 2);

        // assert
        list.Should().Be(expectedList);
    }
    
    [Test]
    public void ToWikiList_ShouldMakeNumericList_SectionIsExactMultipleOfPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(6);
        string expectedList = $"""
                               == 1 ==
                               # [[{pages[0].Title}]]
                               # [[{pages[1].Title}]]
                               == 2 ==
                               # [[{pages[2].Title}]]
                               # [[{pages[3].Title}]]
                               == 3 ==
                               # [[{pages[4].Title}]]
                               # [[{pages[5].Title}]]
                               
                               """;
        
        // act
        string list = pages.ToWikiList(true, 2);

        // assert
        list.Should().Be(expectedList);
    }
    
    
    [Test]
    public void ToWikiList_ShouldMakeBulletList_SectionIsLargerThanPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        string expectedList = $"""
                               == 1 ==
                               * [[{pages[0].Title}]]
                               * [[{pages[1].Title}]]
                               * [[{pages[2].Title}]]
                               * [[{pages[3].Title}]]
                               * [[{pages[4].Title}]]
                               
                               """;
        
        // act
        string list = pages.ToWikiList(false, 6);

        // assert
        list.Should().Be(expectedList);
    }
    
    [Test]
    public void ToWikiList_ShouldMakeBulletList_SectionIsSmallerThanPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(5);
        string expectedList = $"""
                               == 1 ==
                               * [[{pages[0].Title}]]
                               * [[{pages[1].Title}]]
                               == 2 ==
                               * [[{pages[2].Title}]]
                               * [[{pages[3].Title}]]
                               == 3 ==
                               * [[{pages[4].Title}]]

                               """;
        
        // act
        string list = pages.ToWikiList(false, 2);

        // assert
        list.Should().Be(expectedList);
    }

    [Test]
    public void ToWikiList_ShouldMakeBulletList_SectionIsExactMultipleOfPages()
    {
        // arrange
        List<WikiPageModel>? pages = Fakers.GetWikiPageModelFaker(ApiRoot, _wikiClientCache).Generate(6);
        string expectedList = $"""
                               == 1 ==
                               * [[{pages[0].Title}]]
                               * [[{pages[1].Title}]]
                               == 2 ==
                               * [[{pages[2].Title}]]
                               * [[{pages[3].Title}]]
                               == 3 ==
                               * [[{pages[4].Title}]]
                               * [[{pages[5].Title}]]
                               
                               """;
        
        // act
        string list = pages.ToWikiList(false, 2);

        // assert
        list.Should().Be(expectedList);
    }
    
    [Test]
    public void ToWikiListAlphabetically_ShouldReturnEmptyString_WhenPagesIsEmpty()
    {
        // arrange
        var pages = new List<WikiPageModel>();

        // act
        string list = pages.ToWikiListAlphabetically(true);

        // assert
        list.Should().Be(string.Empty);
    }
    
    [Test]
    public void ToWikiListAlphabetically_ShouldMakeNumericList()
    {
        // arrange
        var pages = new List<WikiPageModel>
        {
            new("wfwfweaa", ApiRoot, _wikiClientCache),
            new("baa", ApiRoot, _wikiClientCache),
            new("bafwea", ApiRoot, _wikiClientCache),
            new("1aafew", ApiRoot, _wikiClientCache),
            new("aa", ApiRoot, _wikiClientCache),
            new("wfwaa", ApiRoot, _wikiClientCache),
            new("Wfwfweaa", ApiRoot, _wikiClientCache),
            new("Baa", ApiRoot, _wikiClientCache),
            new("Bafwea", ApiRoot, _wikiClientCache),
            new("Aafew", ApiRoot, _wikiClientCache),
            new("Aa", ApiRoot, _wikiClientCache),
            new("Wfwaa", ApiRoot, _wikiClientCache),
        };
        const string expectedList = $"""
                                     == 1 ==
                                     # [[1aafew]]
                                     == A ==
                                     # [[aa]]
                                     # [[Aa]]
                                     # [[Aafew]]
                                     == B ==
                                     # [[baa]]
                                     # [[Baa]]
                                     # [[bafwea]]
                                     # [[Bafwea]]
                                     == W ==
                                     # [[wfwaa]]
                                     # [[Wfwaa]]
                                     # [[wfwfweaa]]
                                     # [[Wfwfweaa]]

                                     """;
        
        // act
        string list = pages.ToWikiListAlphabetically(true);

        // assert
        list.Should().Be(expectedList);
    }
    
    [Test]
    public void ToWikiListAlphabetically_ShouldMakeBulletList()
    {
        // arrange
        var pages = new List<WikiPageModel>
        {
            new("wfwfweaa", ApiRoot, _wikiClientCache),
            new("baa", ApiRoot, _wikiClientCache),
            new("bafwea", ApiRoot, _wikiClientCache),
            new("1aafew", ApiRoot, _wikiClientCache),
            new("aa", ApiRoot, _wikiClientCache),
            new("wfwaa", ApiRoot, _wikiClientCache),
            new("Wfwfweaa", ApiRoot, _wikiClientCache),
            new("Baa", ApiRoot, _wikiClientCache),
            new("Bafwea", ApiRoot, _wikiClientCache),
            new("Aafew", ApiRoot, _wikiClientCache),
            new("Aa", ApiRoot, _wikiClientCache),
            new("Wfwaa", ApiRoot, _wikiClientCache),
        };
        const string expectedList = $"""
                                     == 1 ==
                                     * [[1aafew]]
                                     == A ==
                                     * [[aa]]
                                     * [[Aa]]
                                     * [[Aafew]]
                                     == B ==
                                     * [[baa]]
                                     * [[Baa]]
                                     * [[bafwea]]
                                     * [[Bafwea]]
                                     == W ==
                                     * [[wfwaa]]
                                     * [[Wfwaa]]
                                     * [[wfwfweaa]]
                                     * [[Wfwfweaa]]

                                     """;
        
        // act
        string list = pages.ToWikiListAlphabetically(false);

        // assert
        list.Should().Be(expectedList);
    }
}