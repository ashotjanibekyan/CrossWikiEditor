using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services;

namespace CrossWikiEditor.Repositories;

public interface IProfileRepository
{
    Profile? Get(int id);
    int Insert(Profile profile);
    void Update(Profile profile);
    List<Profile> GetAll();
    void Delete(int id);
}

public class ProfileRepository : IProfileRepository
{
    private readonly string _connectionString;
    private readonly IStringEncryptionService _stringEncryptionService;

    public ProfileRepository(string connectionString, IStringEncryptionService stringEncryptionService)
    {
        _connectionString = connectionString;
        _stringEncryptionService = stringEncryptionService;
        CreateProfilesTable();
    }
    
    private void CreateProfilesTable()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS Profile (
                                        Id INTEGER PRIMARY KEY,
                                        Username TEXT NOT NULL,
                                        EncryptedPassword BLOB,
                                        IsPasswordSaved INTEGER NOT NULL,
                                        DefaultSettingsPath TEXT,
                                        Notes TEXT
                                    );";

            command.ExecuteNonQuery();
        }

        connection.Close();
    }

    public Profile? Get(int id)
    {
        Profile? profile = null;
        
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = @"
                    SELECT * FROM Profile
                    WHERE Id = @Id;
                ";

            command.Parameters.AddWithValue("@Id", id);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    profile = new Profile
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = Convert.ToString(reader["Username"]),
                        Password = _stringEncryptionService.DecryptStringFromBytes((byte[])reader["EncryptedPassword"]),
                        IsPasswordSaved = Convert.ToBoolean(reader["IsPasswordSaved"]),
                        DefaultSettingsPath = Convert.ToString(reader["DefaultSettingsPath"]),
                        Notes = Convert.ToString(reader["Notes"])
                    };
                }
            }
        }

        connection.Close();

        return profile;
    }

    public int Insert(Profile profile)
    {
        var result = 0;
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"
                    INSERT INTO Profile (Username, EncryptedPassword, IsPasswordSaved, DefaultSettingsPath, Notes)
                    VALUES (@Username, @EncryptedPassword, @IsPasswordSaved, @DefaultSettingsPath, @Notes);
                ";
                
                var encript = _stringEncryptionService.EncryptStringToBytes(profile.Password);

                command.Parameters.AddWithValue("@Username", profile.Username);
                command.Parameters.AddWithValue("@EncryptedPassword", profile.Password is null ? DBNull.Value : _stringEncryptionService.EncryptStringToBytes(profile.Password));
                command.Parameters.AddWithValue("@IsPasswordSaved", profile.IsPasswordSaved);
                command.Parameters.AddWithValue("@DefaultSettingsPath", profile.DefaultSettingsPath);
                command.Parameters.AddWithValue("@Notes", profile.Notes);

                result = command.ExecuteNonQuery();
            }

            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public void Update(Profile profile)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = @"
                    UPDATE Profile
                    SET Username = @Username,
                        EncryptedPassword = @EncryptedPassword,
                        IsPasswordSaved = @IsPasswordSaved,
                        DefaultSettingsPath = @DefaultSettingsPath,
                        Notes = @Notes
                    WHERE Id = @Id;
                ";

            command.Parameters.AddWithValue("@Username", profile.Username);
            command.Parameters.AddWithValue("@EncryptedPassword", profile.Password is null ? DBNull.Value : _stringEncryptionService.EncryptStringToBytes(profile.Password));
            command.Parameters.AddWithValue("@IsPasswordSaved", profile.IsPasswordSaved);
            command.Parameters.AddWithValue("@DefaultSettingsPath", profile.DefaultSettingsPath);
            command.Parameters.AddWithValue("@Notes", profile.Notes);
            command.Parameters.AddWithValue("@Id", profile.Id);

            command.ExecuteNonQuery();
        }

        connection.Close();
    }
    
    public List<Profile> GetAll()
    {
        var values = new List<Profile>();

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = "SELECT * FROM Profile;";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var encryptedPassword = reader["EncryptedPassword"];
                    var profile = new Profile
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = Convert.ToString(reader["Username"]),
                        Password = encryptedPassword is byte[] bytes ? _stringEncryptionService.DecryptStringFromBytes(bytes) : null,
                        IsPasswordSaved = Convert.ToBoolean(reader["IsPasswordSaved"]),
                        DefaultSettingsPath = Convert.ToString(reader["DefaultSettingsPath"]),
                        Notes = Convert.ToString(reader["Notes"])
                    };

                    values.Add(profile);
                }
            }
        }

        connection.Close();

        return values;
    }
    
    public void Delete(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using (var command = new SQLiteCommand(connection))
        {
            command.CommandText = @"
                    DELETE FROM Profile
                    WHERE Id = @Id;
                ";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
        connection.Close();
    }
}