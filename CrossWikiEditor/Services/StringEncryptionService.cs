using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CrossWikiEditor.Services;

public interface IStringEncryptionService
{
    byte[] EncryptStringToBytes(string plainText);
    string DecryptStringFromBytes(byte[] encryptedBytes);
}

public sealed class StringEncryptionService : IStringEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public StringEncryptionService(byte[] key, byte[] iv)
    {
        _key = key;
        _iv = iv;
    }
    
    public byte[] EncryptStringToBytes(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return Array.Empty<byte>();
        }
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            cs.Write(data, 0, data.Length);
        }
        var encryptedBytes = ms.ToArray();
        return encryptedBytes;
    }

    public string DecryptStringFromBytes(byte[] encryptedBytes)
    {
        if (encryptedBytes.Length == 0)
        {
            return string.Empty;
        }
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
    
    public static (byte[] Key, byte[] IV) GenerateKeyAndIv(string passphrase, byte[] salt, int keySize, int ivSize, int iterations)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(passphrase, salt, iterations);
        var key = deriveBytes.GetBytes(keySize);
        var iv = deriveBytes.GetBytes(ivSize);
        return (key, iv);
    }
}