namespace WikiClient.Actions;

public sealed class QueryActionBuilder
{
    private MetaTokenType _tokenType;
    private List<string> _titles;
    
    public QueryActionBuilder WithMeta(MetaTokenType tokenType)
    {
        _tokenType = tokenType;
        return this;
    }

    public QueryActionBuilder WithTitles(List<string> titles)
    {
        _titles = titles;
        return this;
    }

    public string Build()
    {
        return $"action=query&meta=tokens&type={_tokenType.ToString().ToLower()}";
    }
}