namespace CrossWikiEditor.Core.ListProviders.BaseListProviders;

public interface IListProvider
{
    string Title { get; }
    string ParamTitle { get; }
    string Param { get; set; }
    bool CanMake { get; }
}

public abstract class ListProviderBase : IListProvider
{
    public abstract string Title { get; }
    public abstract string ParamTitle { get; }
    public virtual string Param { get; set; } = string.Empty;
    public virtual bool CanMake => !string.IsNullOrEmpty(Param);
}