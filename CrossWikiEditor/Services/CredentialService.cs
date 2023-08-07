using System;

namespace CrossWikiEditor.Services;

public interface ICredentialService
{
    bool SavePassword(string username, string password);
}

public class CredentialService : ICredentialService
{
    public bool SavePassword(string username, string password)
    {
        Console.WriteLine(username + " " + password);
        return true;
    }
}