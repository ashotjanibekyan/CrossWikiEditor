using Bogus;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.Tests;

public static class Fakers
{
    private static Faker _faker = new();
    
    public static Faker<Profile> ProfileFaker = new Faker<Profile>()
        .RuleFor(p => p.DefaultSettingsPath, f => f.System.FilePath())
        .RuleFor(p => p.IsPasswordSaved, f => f.Random.Bool())
        .RuleFor(p => p.Id, f => f.UniqueIndex)
        .RuleFor(p => p.Password, f => f.Internet.Password())
        .RuleFor(p => p.Notes, f => f.Random.Words())
        .RuleFor(p => p.Username, f => f.Internet.UserName());

    public static Faker<WikiPageModel> WikiPageModelFaker = new Faker<WikiPageModel>()
        .CustomInstantiator(f => new WikiPageModel("", 0))
        .RuleFor(p => p.WikiPage, f => null)
        .RuleFor(p => p.Title, f => f.Random.Word())
        .RuleFor(p => p.NamespaceId, f => f.Random.Int(0, 20));
    
    public static List<string> WordsFaker(int n) => _faker.Random.WordsArray(n).ToList();
}