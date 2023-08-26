using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossWikiEditor.Generators;
using CrossWikiEditor.Models;
using CrossWikiEditor.Utils;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace CrossWikiEditor.Services.WikiServices;

public interface IPageService
{
    Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, bool includeHidden = true, bool onlyHidden = false);
    Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0);
    Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName);
    Task<Result<WikiPageModel>> ConvertToTalk(WikiPageModel page);
    Task<Result<List<WikiPageModel>>> ConvertToTalk(List<WikiPageModel> pages);
    Task<Result<WikiPageModel>> ConvertToSubject(WikiPageModel page);
    Task<Result<List<WikiPageModel>>> ConvertToSubject(List<WikiPageModel> pages);
}

public sealed class PageService : IPageService
{
    private readonly IWikiClientCache _wikiClientCache;
    private readonly IUserPreferencesService _userPreferencesService;

    public PageService(IWikiClientCache wikiClientCache, IUserPreferencesService userPreferencesService)
    {
        _wikiClientCache = wikiClientCache;
        _userPreferencesService = userPreferencesService;
    }
    public async Task<Result<List<WikiPageModel>>> GetCategoriesOf(string apiRoot, string pageName, bool includeHidden = true, bool onlyHidden = false)
    {
        try
        {
            if (!includeHidden && onlyHidden)
            {
                return Result<List<WikiPageModel>>.Success(new List<WikiPageModel>());
            }
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoriesGenerator(site, pageName)
            {
                HiddenCategoryFilter = PropertyFilterOption.Disable
            };
            if (!includeHidden)
            {
                catGen.HiddenCategoryFilter = PropertyFilterOption.WithoutProperty;
            }
            if (onlyHidden)
            {
                catGen.HiddenCategoryFilter = PropertyFilterOption.WithProperty;
            }

            List<WikiPage> result = await catGen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> GetPagesOfCategory(string apiRoot, string categoryName, int recursive = 0)
    {
        if (!categoryName.Contains(':'))
        {
            categoryName = $"Category:{categoryName}";
        }
        try
        {
            List<List<WikiPage>> resultByDepth = new();
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot);
            var catGen = new CategoryMembersGenerator(new WikiPage(site, categoryName));
            resultByDepth.Add(await catGen.EnumPagesAsync().ToListAsync());

            for (int i = 0; i < recursive; i++)
            {
                var subCats = resultByDepth[i].Where(x => x.NamespaceId == BuiltInNamespaces.Category).ToList();
                var temp = new List<WikiPage>();
                foreach (WikiPage subCat in subCats)
                {
                    catGen = new CategoryMembersGenerator(new WikiPage(site, subCat.Title));
                    temp.AddRange(await catGen.EnumPagesAsync().ToListAsync());
                }
                resultByDepth.Add(temp);
            }
            
            return Result<List<WikiPageModel>>.Success(resultByDepth.SelectMany(list => list).Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> FilesOnPage(string apiRoot, string pageName)
    {
        try
        {
            WikiSite site = await _wikiClientCache.GetWikiSite(apiRoot);
            var gen = new FilesGenerator(site, pageName);
            List<WikiPage> result = await gen.EnumPagesAsync().ToListAsync();
            return Result<List<WikiPageModel>>.Success(result.Select(x => new WikiPageModel(x)).ToList());
        }
        catch (Exception e)
        {
            return Result<List<WikiPageModel>>.Failure(e.Message);
        }
    }

    public async Task<Result<WikiPageModel>> ConvertToTalk(WikiPageModel page)
    {
        try
        {
            if (page.WikiPage is null)
            {
                UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
                page.WikiPage = new WikiPage(await _wikiClientCache.GetWikiSite(userPrefs.UrlApi()), page.Title);
            }
            return Result<WikiPageModel>.Success(new WikiPageModel(page.WikiPage.ToTalkPage()));
        }
        catch (Exception e)
        {
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> ConvertToTalk(List<WikiPageModel> pages)
    {
        List<WikiPageModel> result = new();
        foreach (WikiPageModel wikiPageModel in pages)
        {
            Result<WikiPageModel> talkPageResult = await ConvertToTalk(wikiPageModel);
            if (talkPageResult is {IsSuccessful: true, Value: not null})
            {
                result.Add(talkPageResult.Value);
            }
        }

        return Result<List<WikiPageModel>>.Success(result);
    }
    
    public async Task<Result<WikiPageModel>> ConvertToSubject(WikiPageModel page)
    {
        try
        {
            if (page.WikiPage is null)
            {
                UserPrefs userPrefs = _userPreferencesService.GetCurrentPref();
                page.WikiPage = new WikiPage(await _wikiClientCache.GetWikiSite(userPrefs.UrlApi()), page.Title);
            }
            return Result<WikiPageModel>.Success(new WikiPageModel(page.WikiPage.ToSubjectPage()));
        }
        catch (Exception e)
        {
            return Result<WikiPageModel>.Failure(e.Message);
        }
    }

    public async Task<Result<List<WikiPageModel>>> ConvertToSubject(List<WikiPageModel> pages)
    {
        List<WikiPageModel> result = new();
        foreach (WikiPageModel wikiPageModel in pages)
        {
            Result<WikiPageModel> subjectPageResult = await ConvertToSubject(wikiPageModel);
            if (subjectPageResult is {IsSuccessful: true, Value: not null})
            {
                result.Add(subjectPageResult.Value);
            }
        }

        return Result<List<WikiPageModel>>.Success(result);
    }
}