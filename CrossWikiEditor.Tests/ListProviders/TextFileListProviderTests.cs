using System.Text;

namespace CrossWikiEditor.Tests.ListProviders;

public class TextFileListProviderTests : ListProvidersBaseTest<TextFileListProvider>
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new TextFileListProvider(_fileDialogService, _systemService, _userPreferencesService, _wikiClientCache);
    }

    [Test]
    public async Task MakeList_ShouldReadBulletWikiList()
    {
        // arrange
        string text = """
                      *[[title1]]


                      * [[Category:title (f e )2|few (fewcas)]]


                      *     [[title3]]
                      *     [[titl e3|display]]
                      """;
        SetupForSingleFile(text);
        
        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value[0].Title.Should().Be("title1");
        result.Value[1].Title.Should().Be("Category:title (f e )2");
        result.Value[2].Title.Should().Be("title3");
        result.Value[3].Title.Should().Be("titl e3");
    }

    [Test]
    public async Task MakeList_ShouldReadNumberedWikiList()
    {
        // arrange
        string text = """
                      #[[title1]]
                      
                      
                      # [[Category:title (f e )2|few (fewcas)]]
                      
                      
                      #     [[title3]]
                      #     [[titl e3|display]]
                      """;
        SetupForSingleFile(text);
        
        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value[0].Title.Should().Be("title1");
        result.Value[1].Title.Should().Be("Category:title (f e )2");
        result.Value[2].Title.Should().Be("title3");
        result.Value[3].Title.Should().Be("titl e3");
    }

    [Test]
    public async Task MakeList_ShouldReadPlainList()
    {
        // arrange
        string text = """
                      title1
                      title2
                      
                      
                      title3
                      """;
        SetupForSingleFile(text);
        
        // act
        await _sut.GetAdditionalParams();
        Result<List<WikiPageModel>> result = await _sut.MakeList();

        // assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value[0].Title.Should().Be("title1");
        result.Value[1].Title.Should().Be("title2");
        result.Value[2].Title.Should().Be("title3");
    }

    [Test]
    public async Task GetAdditionalParams_ShouldMakeCanMakeTrue_WhenFileIsSelected()
    {
        // arrange
        _fileDialogService
            .OpenFilePickerAsync("Select text files to extract pages", true)
            .Returns(new[] {"some/path/text.txt"});

        // act
        await _sut.GetAdditionalParams();

        // assert
        await _fileDialogService.Received(1).OpenFilePickerAsync("Select text files to extract pages", true);
        _sut.CanMake.Should().BeTrue();
    }

    [Test]
    public async Task GetAdditionalParams_ShouldNotMakeCanMakeTrue_WhenFileIsSelected()
    {
        // arrange
        _fileDialogService
            .OpenFilePickerAsync("Select text files to extract pages", true)
            .Returns(null as string[]);

        // act
        await _sut.GetAdditionalParams();

        // assert
        await _fileDialogService.Received(1).OpenFilePickerAsync("Select text files to extract pages", true);
        _sut.CanMake.Should().BeFalse();
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Title.Should().Be("Text file");
        _sut.ParamTitle.Should().Be(string.Empty);
    }

    private void SetupForSingleFile(string text)
    {
        _fileDialogService
            .OpenFilePickerAsync("Select text files to extract pages", true)
            .Returns(new[] {"path/to/file1/text.txt"});
        _systemService
            .ReadAllTextAsync("path/to/file1/text.txt", Encoding.Default)
            .Returns(text);
    }
}