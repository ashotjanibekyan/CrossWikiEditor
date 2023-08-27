using System.Threading.Tasks;

namespace CrossWikiEditor;

public interface IAsyncInitialization
{
    Task InitAsync { get; }
}