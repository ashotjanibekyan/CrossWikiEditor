using System.Collections.Generic;
using CrossWikiEditor.Core.Models;

namespace CrossWikiEditor.Core.Repositories;

public interface IProfileRepository
{
    Profile? Get(int id);
    int Insert(Profile profile);
    void Update(Profile profile);
    List<Profile> GetAll();
    void Delete(int id);
}