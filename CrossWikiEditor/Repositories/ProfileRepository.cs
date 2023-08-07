using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CrossWikiEditor.Models;

namespace CrossWikiEditor.Repositories;

public interface IProfileRepository
{
    Profile? Get(int id);
    int Insert(Profile profile);
    List<Profile> GetAll();
}

public class ProfileRepository : IProfileRepository
{
    private readonly string _connectionString;

    public ProfileRepository(string connectionString)
    {
        _connectionString = connectionString;
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
                    INSERT INTO Profile (Username, IsPasswordSaved, DefaultSettingsPath, Notes)
                    VALUES (@Username, @IsPasswordSaved, @DefaultSettingsPath, @Notes);
                ";

                command.Parameters.AddWithValue("@Username", profile.Username);
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
                    var profile = new Profile
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = Convert.ToString(reader["Username"]),
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
}