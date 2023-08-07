using System.Collections.Generic;
using System.Threading.Tasks;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.Services;

public interface IProfileService
{
    Task<List<Profile>> GetAllProfiles();
}

public class ProfileService : IProfileService
{
    public Task<List<Profile>> GetAllProfiles()
    {
        throw new System.NotImplementedException();
    }
}