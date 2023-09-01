using System.Threading.Tasks;

namespace CrossWikiEditor.ListProviders.BaseListProviders;

public interface INeedAdditionalParamsListProvider : IListProvider
{
    Task GetAdditionalParams();
}