using Autofac;
using CrossWikiEditor.Core.ListProviders;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

namespace CrossWikiEditor.AutofacModules;

public sealed class ListProvidersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // TODO: CheckWiki error
        // TODO: CheckWiki error (number)
        // TODO: Database dump
        builder.RegisterType<AllCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<AllFilesListProvider>().As<IListProvider>();
        builder.RegisterType<AllPagesListProvider>().As<IListProvider>();
        builder.RegisterType<AllPagesNoRedirectsListProvider>().As<IListProvider>();
        builder.RegisterType<AllPagesWithPrefixListProvider>().As<IListProvider>();
        builder.RegisterType<AllRedirectsListProvider>().As<IListProvider>();
        builder.RegisterType<AllUsersListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageNoHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoriesOnPageOnlyHiddenCategoriesListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursive1LevelListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveListProvider>().As<IListProvider>();
        builder.RegisterType<CategoryRecursiveUserDefinedLevelListProviderBase>().As<IListProvider>();
        builder.RegisterType<DisambiguationPagesListProvider>().As<IListProvider>();
        builder.RegisterType<FilesOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<GoogleSearchListProvider>().As<IListProvider>();
        builder.RegisterType<HtmlScraperListProvider>().As<IListProvider>();
        builder.RegisterType<ImageFileLinksListProvider>().As<IListProvider>();
        builder.RegisterType<LinkSearchListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageBlueListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<LinksOnPageRedListProvider>().As<IListProvider>();
        builder.RegisterType<MyWatchlistListProvider>().As<IListProvider>();
        builder.RegisterType<NewPagesListProvider>().As<IListProvider>();
        builder.RegisterType<PagesWithPropListProvider>().As<IListProvider>();
        builder.RegisterType<PagesWithoutLanguageLinksListProvider>().As<IListProvider>();
        builder.RegisterType<PagesWithoutLanguageLinksNoRedirectsListProvider>().As<IListProvider>();
        builder.RegisterType<ProtectedPagesListProvider>().As<IListProvider>();
        builder.RegisterType<RandomListProvider>().As<IListProvider>();
        builder.RegisterType<RecentChangesListProvider>().As<IListProvider>();
        builder.RegisterType<TextFileListProvider>().As<IListProvider>();
        builder.RegisterType<TransclusionsOnPageListProvider>().As<IListProvider>();
        builder.RegisterType<UserContributionsListProvider>().As<IListProvider>();
        builder.RegisterType<WhatLinksHereListProvider>().As<IListProvider>();
        builder.RegisterType<WhatTranscludesHereAllNsListProvider>().As<IListProvider>();
        builder.RegisterType<WhatTranscludesHereListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTextAllNsListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTextListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTitleAllNsListProvider>().As<IListProvider>();
        builder.RegisterType<WikiSearchInTitleListProvider>().As<IListProvider>();

        builder.RegisterType<TextFileListProvider>();
    }
}