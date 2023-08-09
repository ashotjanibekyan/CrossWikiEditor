namespace WikiClient.Actions;

public class QueryTokensMeta : QueryMetaBase
{
    public QueryTokensMeta(MetaTokenType tokenType)
    {
    }

    public override string ApiString => "tokens";
}

public enum MetaTokenType
{
    allvalues,
    createaccount,
    csrf,
    deleteglobalaccount,
    login,
    patrol,
    rollback,
    setglobalaccountstatus,
    userrights,
    watch
}