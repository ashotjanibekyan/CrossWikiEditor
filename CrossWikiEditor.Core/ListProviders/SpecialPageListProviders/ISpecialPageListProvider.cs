using CrossWikiEditor.Core.ListProviders.BaseListProviders;

namespace CrossWikiEditor.Core.ListProviders.SpecialPageListProviders;

public interface ISpecialPageListProvider : IListProvider
{
    int NamespaceId { get; set; }
    bool NeedsNamespace { get; }
}