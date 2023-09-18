using CrossWikiEditor.Core.ViewModels.ControlViewModels;

namespace CrossWikiEditor.Tests.ViewModels.ControlViewModels;

public class StartViewModelTests : BaseTest
{
    private StartViewModel _sut;
    
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
        _sut = new StartViewModel(_messenger);
    }

    [Test]
    public void StartCommand_ShouldSendStartBotMessage()
    {
        // arrange

        // act
        _sut.StartCommand.Execute(null);

        // assert
        _messenger.Received(1).Send(Arg.Any<StartBotMessage>());
    }

    [Test]
    public void StopCommand_ShouldSendStopBotMessage()
    {
        // arrange

        // act
        _sut.StopCommand.Execute(null);

        // assert
        _messenger.Received(1).Send(Arg.Any<StopBotMessage>());
    }
}