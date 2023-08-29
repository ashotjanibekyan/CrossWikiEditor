using System.Threading.Tasks;

namespace CrossWikiEditor.ListProviders;

public interface INeedAdditionalParamsListProvider : IListProvider
{
    Task GetAdditionalParams();
}