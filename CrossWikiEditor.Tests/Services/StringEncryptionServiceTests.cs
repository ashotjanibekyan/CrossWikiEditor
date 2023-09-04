using System.Security.Cryptography;
using CrossWikiEditor.Core.Services;

namespace CrossWikiEditor.Tests.Services;

public class StringEncryptionServiceTests
{
    [Test]
    [Combinatorial]
    public void ShouldDecryptEncrypted_WhenKeyAndIvAreTheSame(
        [Values("", "a", "this is a long passphrase", "this is a very long passphrase")]
        string passphrase,
        [Values("", "  ",
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum")]
        string message)
    {
        // arrange
        (byte[] key, byte[] iv) = StringEncryptionService.GenerateKeyAndIv(passphrase);
        var sut = new StringEncryptionService(key, iv);
        byte[]? encrypted = sut.EncryptStringToBytes(message);

        // act
        (byte[] key1, byte[] iv1) = StringEncryptionService.GenerateKeyAndIv(passphrase);
        var sut1 = new StringEncryptionService(key1, iv1);
        string? decrypted = sut1.DecryptStringFromBytes(encrypted);

        // assert
        decrypted.Should().Be(message);
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase(" fwefwegwbv cwef we ")]
    [TestCase(
        "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum")]
    public void ShouldNotDecryptEncrypted_WhenPassphraseIsDifferent(string message)
    {
        // arrange
        (byte[] key, byte[] iv) = StringEncryptionService.GenerateKeyAndIv("passphrase1");
        var sut = new StringEncryptionService(key, iv);
        byte[]? encrypted = sut.EncryptStringToBytes(message);

        // act
        (byte[] key1, byte[] iv1) = StringEncryptionService.GenerateKeyAndIv("passphrase2");
        var sut1 = new StringEncryptionService(key1, iv1);


        Action decrypting = () => sut1.DecryptStringFromBytes(encrypted);

        // assert
        decrypting
            .Should()
            .Throw<CryptographicException>()
            .WithMessage("Padding is invalid and cannot be removed.");
    }
}