using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Core.Repositories;

file sealed class JsonProfile
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsPasswordSaved { get; set; }
    public byte[] Password { get; set; } = [];
    public string DefaultSettingsPath { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

file static class Mapper
{
    public static JsonProfile ProfileToJsonProfile(Profile profile, IStringEncryptionService stringEncryptionService)
    {
        return new JsonProfile
        {
            Id = profile.Id,
            DefaultSettingsPath = profile.DefaultSettingsPath,
            IsPasswordSaved = profile.IsPasswordSaved,
            Notes = profile.Notes,
            Password = stringEncryptionService.EncryptStringToBytes(profile.Password),
            Username = profile.Username
        };
    }

    public static Profile JsonProfileToProfile(JsonProfile realmProfile, IStringEncryptionService stringEncryptionService)
    {
        return new Profile
        {
            Id = realmProfile.Id,
            DefaultSettingsPath = realmProfile.DefaultSettingsPath,
            IsPasswordSaved = realmProfile.IsPasswordSaved,
            Password = stringEncryptionService.DecryptStringFromBytes(realmProfile.Password),
            Notes = realmProfile.Notes,
            Username = realmProfile.Username
        };
    }
}

/// <summary>
///     Realm is great but we use a tiny fraction of its capabilities and it is responsible for 30% of the final filesize
///     of the app.
///     Write the content on a file instead for now.
/// </summary>
public sealed class SimpleJsonProfileRepository : IProfileRepository
{
    private const string JsonName = "profiles.json";
    private readonly object _profileJsonLock = new();
    private readonly IStringEncryptionService _stringEncryptionService;

    /// <summary>
    ///     Realm is great but we use a tiny fraction of its capabilities and it is responsible for 30% of the final filesize
    ///     of the app.
    ///     Write the content on a file instead for now.
    /// </summary>
    public SimpleJsonProfileRepository(IStringEncryptionService stringEncryptionService)
    {
        _stringEncryptionService = stringEncryptionService;
    }

    public Profile? Get(int id)
    {
        List<Profile> profiles = GetAll();
        return profiles.Find(p => p.Id == id);
    }

    public int Insert(Profile profile)
    {
        List<Profile> profiles = GetAll();
        profile.Id = profiles.Count != 0 ? profiles.Max(p => p.Id) + 1 : 0;
        profiles.Add(profile);
        SaveAll(profiles);
        return 1;
    }

    public void Update(Profile profile)
    {
        List<Profile> profiles = GetAll();
        Profile? p = profiles.Find(p => p.Id == profile.Id);
        if (p is not null)
        {
            profiles.Remove(p);
            profiles.Add(profile);
        }

        SaveAll(profiles);
    }

    public List<Profile> GetAll()
    {
        lock (_profileJsonLock)
        {
            if (!File.Exists(JsonName))
            {
                SaveAll([]);
            }

            string json = File.ReadAllText(JsonName);
            List<JsonProfile>? jsonProfiles = JsonSerializer.Deserialize<List<JsonProfile>>(json);
            return jsonProfiles is null ? [] : jsonProfiles.ConvertAll(p => Mapper.JsonProfileToProfile(p, _stringEncryptionService));
        }
    }

    public void Delete(int id)
    {
        List<Profile> profiles = GetAll();
        Profile? p = profiles.Find(p => p.Id == id);
        if (p is not null)
        {
            profiles.Remove(p);
        }

        SaveAll(profiles);
    }

    private void SaveAll(List<Profile> profiles)
    {
        lock (_profileJsonLock)
        {
            List<JsonProfile> jsonProfiles = profiles.ConvertAll(p => Mapper.ProfileToJsonProfile(p, _stringEncryptionService));
            string json = JsonSerializer.Serialize(jsonProfiles, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(JsonName, json);
        }
    }
}