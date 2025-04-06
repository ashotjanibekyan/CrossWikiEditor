using System.Threading.Tasks;

namespace CrossWikiEditor.Core;

public interface IAsyncInitialization
{
    Task InitAsync { get; }
}