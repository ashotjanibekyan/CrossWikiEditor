using System.Text;

namespace WikiClient.Actions;

public sealed class QueryAction
{
    private List<QueryPropertyBase> _queryProperties;
    private List<QueryMetaBase> _queryMetas;
    private List<QueryListBase> _queryLists;

    public QueryAction()
    {
    }

    public QueryAction(List<QueryPropertyBase> queryProperties, List<QueryMetaBase> queryMetas, List<QueryListBase> queryLists)
    {
        _queryProperties = queryProperties;
        _queryMetas = queryMetas;
        _queryLists = queryLists;
    }

    private string GetApiQuery()
    {
        var sb = new StringBuilder();
        sb.Append("query");

        foreach (QueryMetaBase? meta in _queryMetas)
        {
        }

        return sb.ToString();
    }

    public async Task Execute()
    {
    }

    public static object WithMeta(MetaTokenType login)
    {
        throw new NotImplementedException();
    }
}

public class QueryListBase
{
}