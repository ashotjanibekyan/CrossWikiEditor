using System;
using System.Threading.Tasks;

namespace CrossWikiEditor.Services;

public interface ICredentialService
{
    Task<string> GetLoginToken();
}

public class CredentialService : ICredentialService
{
    public Task<string> GetLoginToken()
    {
        return Task.FromResult("");
    }
}