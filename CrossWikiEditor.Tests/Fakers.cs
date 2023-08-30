using Bogus;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.ViewModels;

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
    
    public static Faker<WikiPageModel> GetWikiPageModelFaker(string apiRoot, IWikiClientCache wikiClientCache) =>
        new Faker<WikiPageModel>()
            .CustomInstantiator(f => new WikiPageModel(f.Random.Word(), apiRoot, wikiClientCache))
            .RuleFor(p => p.NamespaceId, f => f.Random.Int(0, 20));
    
    public static Faker<WikiNamespace> WikiNamespaceFaker = new Faker<WikiNamespace>()
        .CustomInstantiator(f => new WikiNamespace(f.UniqueIndex, f.Random.Word(), false));
}