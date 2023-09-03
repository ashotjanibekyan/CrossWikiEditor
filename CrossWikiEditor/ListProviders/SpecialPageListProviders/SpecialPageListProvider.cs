﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.ListProviders.BaseListProviders;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using CrossWikiEditor.Utils;

namespace CrossWikiEditor.ListProviders.SpecialPageListProviders;

public class SpecialPageListProvider(
    IDialogService dialogService, 
    IViewModelFactory viewModelFactory) : UnlimitedListProviderBase, INeedAdditionalParamsListProvider
{
    private ISpecialPageListProvider? _selectedListProvider;
    public override string Title => "Special page";
    public override string ParamTitle => string.Empty;
    public override bool CanMake => _selectedListProvider != null;

    public async Task GetAdditionalParams()
    {
        _selectedListProvider = await dialogService.ShowDialog<ISpecialPageListProvider>(await viewModelFactory.GetSpecialPageListProviderSelectorViewModel());
    }

    public override async Task<Result<List<WikiPageModel>>> MakeList()
    {
        if (_selectedListProvider is not {CanMake: true})
        {
            return Result<List<WikiPageModel>>.Failure("");
        }

        return _selectedListProvider switch
        {
            ILimitedListProvider limitedListProvider => await limitedListProvider.MakeList(await limitedListProvider.GetLimit()),
            IUnlimitedListProvider unlimitedListProvider => await unlimitedListProvider.MakeList(),
            _ => Result<List<WikiPageModel>>.Failure("")
        };
    }
}
