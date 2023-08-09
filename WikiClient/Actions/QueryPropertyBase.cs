namespace WikiClient.Actions;

public abstract class QueryPropertyBase : IApiEntity
{
    public abstract string ApiString { get; }
}