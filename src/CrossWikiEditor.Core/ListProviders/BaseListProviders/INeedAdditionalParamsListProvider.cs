using System.Threading.Tasks;

namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public interface INeedAdditionalParamsListProvider : IListProvider
{
    Task GetAdditionalParams();
}