using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;
using Realms;
using Realms.Schema;

namespace CrossWikiEditor.Repositories;

class RealmProfile : RealmObject
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Username { get; set; }
    public bool IsPasswordSaved { get; set; }
    public byte[] Password { get; set; }
    public string DefaultSettingsPath { get; set; }
    public string Notes { get; set; }
}

public class RealmProfileRepository : IProfileRepository
{
    private readonly IStringEncryptionService _stringEncryptionService;
    private readonly RealmConfigurationBase _realmConfiguration;

    public RealmProfileRepository(string appData, IStringEncryptionService stringEncryptionService)
    {
        _stringEncryptionService = stringEncryptionService;

        _realmConfiguration = new RealmConfiguration(Path.Combine(appData, "profiles.realm"))
        {
            Schema =new RealmSchema.Builder
            {
                new ObjectSchema.Builder(typeof(RealmProfile))
            }
        };
    }
    
    public Profile? Get(int id)
    {
        using var realm = Realm.GetInstance(_realmConfiguration);
        var profile = realm.Find<RealmProfile>(id);
        return profile is null ? null : RealmProfileToProfile(profile);
    }

    public int Insert(Profile profile)
    {
        var realmProfile = ProfileToRealmProfile(profile);
        using var realm = Realm.GetInstance(_realmConfiguration);

        var all = realm.All<RealmProfile>().ToList();
        realmProfile.Id = all.Any() ? all.Max(p => p.Id) + 1 : 0;
        
        realm.Write(() =>
        {
            realm.Add(realmProfile);
        });

        return realmProfile.Id;
    }

    public void Update(Profile profile)
    {
        using var realm = Realm.GetInstance(_realmConfiguration);
        var realmProfile = ProfileToRealmProfile(profile);
        realm.Write(() =>
        {
            realm.Add(realmProfile, update: true);
        });
    }

    public List<Profile> GetAll()
    {
        using var realm = Realm.GetInstance(_realmConfiguration);
        var result = realm.All<RealmProfile>().ToList();
        return result.Select(RealmProfileToProfile).ToList();
    }

    public void Delete(int id)
    {
        using var realm = Realm.GetInstance(_realmConfiguration);
        var profile = realm.Find<RealmProfile>(id);
        if (profile != null)
        {
            realm.Write(() =>
            {
                realm.Remove(profile);
            });
        }
    }
    
    private RealmProfile ProfileToRealmProfile(Profile profile)
    {
        return new RealmProfile
        {
            Id = profile.Id,
            DefaultSettingsPath = profile.DefaultSettingsPath,
            IsPasswordSaved = profile.IsPasswordSaved,
            Notes = profile.Notes,
            Password = _stringEncryptionService.EncryptStringToBytes(profile.Password),
            Username = profile.Username
        };
    }

    private Profile RealmProfileToProfile(RealmProfile realmProfile)
    {
        return new Profile
        {
            Id = realmProfile.Id,
            DefaultSettingsPath = realmProfile.DefaultSettingsPath,
            IsPasswordSaved = realmProfile.IsPasswordSaved,
            Password = _stringEncryptionService.DecryptStringFromBytes(realmProfile.Password),
            Notes = realmProfile.Notes,
            Username = realmProfile.Username,
        };
    }
}