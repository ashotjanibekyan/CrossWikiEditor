using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Tests.Services;

public class SettingsServiceTests
{
    private readonly string _oldSettingsDir = "./oldSettings";
    private readonly string _testSettingsPath = "./settings.json";
    private IMessengerWrapper _messenger;

    [SetUp]
    public void Setup()
    {
        _messenger = Substitute.For<IMessengerWrapper>();

        // Clean up any test files from previous runs
        if (File.Exists(_testSettingsPath))
        {
            File.Delete(_testSettingsPath);
        }

        if (Directory.Exists(_oldSettingsDir))
        {
            Directory.Delete(_oldSettingsDir, true);
        }
    }

    [TearDown]
    public void Cleanup()
    {
        // Clean up after tests
        if (File.Exists(_testSettingsPath))
        {
            File.Delete(_testSettingsPath);
        }

        if (Directory.Exists(_oldSettingsDir))
        {
            Directory.Delete(_oldSettingsDir, true);
        }
    }

    [Test]
    public void Constructor_WhenSettingsFileDoesNotExist_ShouldInitializeWithDefaultSettings()
    {
        // Arrange & Act
        var service = new SettingsService(_messenger);

        // Assert
        UserSettings settings = service.GetCurrentSettings();
        settings.Should().NotBeNull();
        settings.Should().BeEquivalentTo(UserSettings.GetDefaultUserSettings());
    }

    [Test]
    public void Constructor_WhenSettingsFileExists_ShouldLoadSettingsFromFile()
    {
        // Arrange
        var expectedSettings = new UserSettings
        {
            UserWiki = new UserWiki("fr", ProjectEnum.Wikisource)
        };

        string json = JsonSerializer.Serialize(expectedSettings);
        File.WriteAllText(_testSettingsPath, json);

        // Act
        var service = new SettingsService(_messenger);

        // Assert
        UserSettings actualSettings = service.GetCurrentSettings();
        actualSettings.Should().NotBeNull();
        actualSettings.UserWiki.LanguageCode.Should().Be("fr");
        actualSettings.UserWiki.Project.Should().Be(ProjectEnum.Wikisource);
    }

    [Test]
    public void Constructor_RegistersMessageHandlersCorrectly()
    {
        // Arrange & Act
        var service = new SettingsService(_messenger);

        // Assert
        _messenger.Received(1).Register(
            Arg.Is(service),
            Arg.Any<MessageHandler<object, LanguageCodeChangedMessage>>());

        _messenger.Received(1).Register(
            Arg.Is(service),
            Arg.Any<MessageHandler<object, ProjectChangedMessage>>());
    }

    [Test]
    public void GetDefaultSettings_ShouldReturnDefaultSettings()
    {
        // Arrange
        var service = new SettingsService(_messenger);

        // Act
        UserSettings defaultSettings = service.GetDefaultSettings();

        // Assert
        defaultSettings.Should().NotBeNull();
        defaultSettings.Should().BeEquivalentTo(UserSettings.GetDefaultUserSettings());
    }

    [Test]
    public void GetSettingsByPath_WithValidPathAndContent_ShouldDeserializeSettings()
    {
        // Arrange
        var service = new SettingsService(_messenger);
        var expectedSettings = new UserSettings
        {
            UserWiki = new UserWiki("de", ProjectEnum.Wikipedia)
        };

        string tempPath = "./temp_settings.json";
        string json = JsonSerializer.Serialize(expectedSettings);
        File.WriteAllText(tempPath, json);

        // Act
        UserSettings? result = service.GetSettingsByPath(tempPath);

        // Assert
        result.Should().NotBeNull();
        result.UserWiki.LanguageCode.Should().Be("de");
        result.UserWiki.Project.Should().Be(ProjectEnum.Wikipedia);

        // Cleanup
        File.Delete(tempPath);
    }

    [Test]
    public void GetSettingsByPath_WithNullOrEmptyPath_ShouldReturnNull()
    {
        // Arrange
        var service = new SettingsService(_messenger);

        // Act
        UserSettings? resultEmpty = service.GetSettingsByPath(string.Empty);

        // Assert
        resultEmpty.Should().BeNull();
    }

    [Test]
    public void SaveCurrentSettings_WhenSettingsFileDoesNotExist_ShouldCreateNewFile()
    {
        // Arrange
        var service = new SettingsService(_messenger);
        UserSettings settings = service.GetCurrentSettings();

        // Act
        service.SaveCurrentSettings();

        // Assert
        File.Exists(_testSettingsPath).Should().BeTrue();

        // Verify content
        string savedJson = File.ReadAllText(_testSettingsPath);
        UserSettings? restoredSettings = JsonSerializer.Deserialize<UserSettings>(savedJson);

        restoredSettings.Should().BeEquivalentTo(settings);
    }

    [Test]
    public void SaveCurrentSettings_WhenSettingsFileExists_ShouldBackupAndCreateNewFile()
    {
        // Arrange
        var initialSettings = new UserSettings
        {
            UserWiki = new UserWiki("es", ProjectEnum.Wikibooks)
        };

        string json = JsonSerializer.Serialize(initialSettings);
        File.WriteAllText(_testSettingsPath, json);

        var service = new SettingsService(_messenger);

        // Act
        service.SaveCurrentSettings();

        // Assert
        File.Exists(_testSettingsPath).Should().BeTrue();
        Directory.Exists(_oldSettingsDir).Should().BeTrue();

        // Check if there's a backup file
        string[] backupFiles = Directory.GetFiles(_oldSettingsDir);
        backupFiles.Should().HaveCount(1);
        backupFiles[0].Should().Contain("settings.json");
    }

    [Test]
    public void GetCurrentSettings_ShouldReturnCurrentSettings()
    {
        // Arrange
        var expectedSettings = new UserSettings
        {
            UserWiki = new UserWiki("it", ProjectEnum.Wiktionary)
        };

        string json = JsonSerializer.Serialize(expectedSettings);
        File.WriteAllText(_testSettingsPath, json);

        var service = new SettingsService(_messenger);

        // Act
        UserSettings result = service.GetCurrentSettings();

        // Assert
        result.Should().NotBeNull();
        result.UserWiki.LanguageCode.Should().Be("it");
        result.UserWiki.Project.Should().Be(ProjectEnum.Wiktionary);
    }

    [Test]
    public void CurrentApiUrl_ShouldReturnUrlFromCurrentSettings()
    {
        // Arrange
        var expectedSettings = new UserSettings
        {
            UserWiki = new UserWiki("en", ProjectEnum.Wikipedia)
        };

        string json = JsonSerializer.Serialize(expectedSettings);
        File.WriteAllText(_testSettingsPath, json);

        var service = new SettingsService(_messenger);

        // Act
        string apiUrl = service.CurrentApiUrl;

        // Assert
        apiUrl.Should().Be(expectedSettings.UserWiki.GetApiUrl());
    }

    [Test]
    public void SetCurrentSettings_ShouldUpdateSettingsAndSendMessage()
    {
        // Arrange
        var service = new SettingsService(_messenger);
        var newSettings = new UserSettings
        {
            UserWiki = new UserWiki("ja", ProjectEnum.Wikivoyage)
        };

        // Act
        service.SetCurrentSettings(newSettings);

        // Assert
        UserSettings currentSettings = service.GetCurrentSettings();
        currentSettings.Should().Be(newSettings);

        _messenger.Received(1).Send(Arg.Is<CurrentSettingsUpdatedMessage>(
            msg => msg.Value == newSettings));
    }

    [Test]
    public void LanguageCodeChangedMessage_ShouldUpdateCurrentSettings()
    {
        // Arrange
        MessageHandler<object, LanguageCodeChangedMessage>? capturedHandler = null;

        _messenger.When(x => x.Register(
            Arg.Any<object>(),
            Arg.Any<MessageHandler<object, LanguageCodeChangedMessage>>())
        ).Do(callInfo => { capturedHandler = callInfo.ArgAt<MessageHandler<object, LanguageCodeChangedMessage>>(1); });

        var service = new SettingsService(_messenger);
        capturedHandler.Should().NotBeNull("because message handler should be registered");

        // Act - manually invoke the handler
        var message = new LanguageCodeChangedMessage("zh");
        capturedHandler(service, message);

        // Assert
        service.GetCurrentSettings().UserWiki.LanguageCode.Should().Be("zh");
    }

    [Test]
    public void ProjectChangedMessage_ShouldUpdateCurrentSettings()
    {
        // Arrange
        MessageHandler<object, ProjectChangedMessage>? capturedHandler = null;

        _messenger.When(x => x.Register(
            Arg.Any<object>(),
            Arg.Any<MessageHandler<object, ProjectChangedMessage>>())
        ).Do(callInfo => { capturedHandler = callInfo.ArgAt<MessageHandler<object, ProjectChangedMessage>>(1); });

        var service = new SettingsService(_messenger);
        capturedHandler.Should().NotBeNull("because message handler should be registered");

        // Act - manually invoke the handler
        var message = new ProjectChangedMessage(ProjectEnum.Commons);
        capturedHandler(service, message);

        // Assert
        service.GetCurrentSettings().UserWiki.Project.Should().Be(ProjectEnum.Commons);
    }
}