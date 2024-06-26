﻿namespace CrossWikiEditor.Core.Services;

public interface IStringEncryptionService
{
    byte[] EncryptStringToBytes(string plainText);
    string DecryptStringFromBytes(byte[] encryptedBytes);
}

public sealed class StringEncryptionService(byte[] key, byte[] iv) : IStringEncryptionService
{
    private static readonly byte[] Salt =
    [
        130, 172, 223, 224, 181, 229, 138, 159, 136, 84, 68, 219, 64, 243, 115, 223, 223, 18, 132, 188, 12, 1, 108, 54, 184, 239, 230, 98, 195, 119,
        226, 97
    ];

    public byte[] EncryptStringToBytes(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            byte[]? data = Encoding.UTF8.GetBytes(plainText);
            cs.Write(data, 0, data.Length);
        }

        byte[]? encryptedBytes = ms.ToArray();
        return encryptedBytes;
    }

    public string DecryptStringFromBytes(byte[] encryptedBytes)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    public static (byte[] Key, byte[] IV) GenerateKeyAndIv(string passphrase)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(passphrase, Salt, 10000, HashAlgorithmName.SHA256);
        byte[]? key = deriveBytes.GetBytes(32);
        byte[]? iv = deriveBytes.GetBytes(16);
        return (key, iv);
    }
}