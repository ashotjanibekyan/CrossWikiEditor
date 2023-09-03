using CrossWikiEditor.ListProviders.BaseListProviders;

namespace CrossWikiEditor.ListProviders.SpecialPageListProviders;

public interface ISpecialPageListProvider : IListProvider
{
    int NamespaceId { get; set; }
    bool NeedsNamespace { get; }
}