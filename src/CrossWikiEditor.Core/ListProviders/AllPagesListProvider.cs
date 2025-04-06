using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;
using WikiClientLibrary.Generators;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class AllPagesListProvider : AllPagesListProviderBase
{
    public AllPagesListProvider(IDialogService dialogService,
        IPageService pageService,
        ISettingsService settingsService,
        IViewModelFactory viewModelFactory) : base(dialogService, pageService, viewModelFactory, settingsService)
    {
    }

    public override string Title => "All Pages";

    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await MakeListBase(limit, PropertyFilterOption.Disable, PropertyFilterOption.Disable);
    }
}