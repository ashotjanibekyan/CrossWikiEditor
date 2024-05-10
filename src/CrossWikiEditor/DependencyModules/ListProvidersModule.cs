using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using Microsoft.Extensions.DependencyInjection;

namespace CrossWikiEditor.DependencyModules;

public static class ListProvidersModule
{
    public static void Register(IServiceCollection services)
    {
        services.AddTransient<IListProvider, AllCategoriesListProvider>();
        services.AddTransient<IListProvider, AllFilesListProvider>();
        services.AddTransient<IListProvider, AllPagesListProvider>();
        services.AddTransient<IListProvider, AllPagesNoRedirectsListProvider>();
        services.AddTransient<IListProvider, AllPagesWithPrefixListProvider>();
        services.AddTransient<IListProvider, AllRedirectsListProvider>();
        services.AddTransient<IListProvider, AllUsersListProvider>();
        services.AddTransient<IListProvider, CategoriesOnPageListProvider>();
        services.AddTransient<IListProvider, CategoriesOnPageNoHiddenCategoriesListProvider>();
        services.AddTransient<IListProvider, CategoriesOnPageOnlyHiddenCategoriesListProvider>();
        services.AddTransient<IListProvider, CategoryListProvider>();
        services.AddTransient<IListProvider, CategoryRecursive1LevelListProvider>();
        services.AddTransient<IListProvider, CategoryRecursiveListProvider>();
        services.AddTransient<IListProvider, CategoryRecursiveUserDefinedLevelListProvider>();
        services.AddTransient<IListProvider, DatabaseDumpListProvider>();
        services.AddTransient<IListProvider, DisambiguationPagesListProvider>();
        services.AddTransient<IListProvider, FilesOnPageListProvider>();
        services.AddTransient<IListProvider, HtmlScraperListProvider>();
        services.AddTransient<IListProvider, ImageFileLinksListProvider>();
        services.AddTransient<IListProvider, LinkSearchListProvider>();
        services.AddTransient<IListProvider, LinksOnPageBlueListProvider>();
        services.AddTransient<IListProvider, LinksOnPageListProvider>();
        services.AddTransient<IListProvider, LinksOnPageRedListProvider>();
        services.AddTransient<IListProvider, MyWatchlistListProvider>();
        services.AddTransient<IListProvider, NewPagesListProvider>();
        services.AddTransient<IListProvider, PagesWithPropListProvider>();
        services.AddTransient<IListProvider, PagesWithoutLanguageLinksListProvider>();
        services.AddTransient<IListProvider, PagesWithoutLanguageLinksNoRedirectsListProvider>();
        services.AddTransient<IListProvider, PetscanListProvider>();
        services.AddTransient<IListProvider, ProtectedPagesListProvider>();
        services.AddTransient<IListProvider, RandomListProvider>();
        services.AddTransient<IListProvider, RecentChangesListProvider>();
        services.AddTransient<IListProvider, TextFileListProvider>();
        services.AddTransient<IListProvider, TransclusionsOnPageListProvider>();
        services.AddTransient<IListProvider, UserContributionsListProvider>();
        services.AddTransient<IListProvider, WhatLinksHereListProvider>();
        services.AddTransient<IListProvider, WhatTranscludesHereAllNsListProvider>();
        services.AddTransient<IListProvider, WhatTranscludesHereListProvider>();
        services.AddTransient<IListProvider, WikiSearchInTextAllNsListProvider>();
        services.AddTransient<IListProvider, WikiSearchInTextListProvider>();
        services.AddTransient<IListProvider, WikiSearchInTitleAllNsListProvider>();
        services.AddTransient<IListProvider, WikiSearchInTitleListProvider>();

        services.AddTransient<TextFileListProvider>();
    }
}