using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CrossWikiEditor.Core.ListProviders.BaseListProviders;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;
using CrossWikiEditor.Core.Services.WikiServices;
using CrossWikiEditor.Core.Utils;

namespace CrossWikiEditor.Core.ListProviders;

public sealed class PetscanListProvider : UnlimitedListProviderBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISettingsService _settingsService;
    private readonly IWikiClientCache _wikiClientCache;

    public PetscanListProvider(IHttpClientFactory httpClientFactory, ISettingsService settingsService, IWikiClientCache wikiClientCache)
    {
        _httpClientFactory = httpClientFactory;
        _settingsService = settingsService;
        _wikiClientCache = wikiClientCache;
    }

    public override string Title => "Petscan";
    public override string ParamTitle => "PSID";

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        try
        {
            var wikiPageModels = new List<WikiPageModel>();
            if (long.TryParse(Param, out long id))
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient("Petscan");
                HttpResponseMessage result = await httpClient.GetAsync($"https://petscan.wmflabs.org/?psid={id}&format=plain");
                if (result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    var titles = content.Split('\n').ToList();
                    wikiPageModels.AddRange(titles.Select(title => new WikiPageModel(title, _settingsService.CurrentApiUrl, _wikiClientCache)));
                }
                else
                {
                    return new Exception($"Could not get the list. \n{await result.Content.ReadAsStringAsync()}");
                }
            }
            else
            {
                return new Exception($"{Param} is not a valid PSID");
            }

            return wikiPageModels;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}