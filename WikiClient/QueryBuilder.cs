namespace WikiClient;

public sealed class QueryBuilder
{
    private string _action;
    private OutputFormat _format;
    
    public QueryBuilder WithAction(string action)
    {
        this._action = action;
        return this;
    }

    public QueryBuilder WithFormat(OutputFormat format)
    {
        _format = format;
        return this;
    }

    public string Build()
    {
        return $"{_action}&format={_format.ToString().ToLower()}&formatversion=2";
    }
}