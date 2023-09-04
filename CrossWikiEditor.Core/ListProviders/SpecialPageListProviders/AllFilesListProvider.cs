using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public class AllFilesListProvider(
    IDialogService dialogService,
    IPageService pageService,
    IUserPreferencesService userPreferencesService) : LimitedListProviderBase(dialogService), ISpecialPageListProvider
{
    public override string Title => "All Files";
    public override string ParamTitle => "Start from";
    public override bool CanMake => true;
    public int NamespaceId { get; set; }
    public bool NeedsNamespace => false;
    public override async Task<Result<List<WikiPageModel>>> MakeList(int limit)
    {
        return await pageService.GetAllFiles(userPreferencesService.GetCurrentPref().UrlApi(), Param, limit);
    }
}