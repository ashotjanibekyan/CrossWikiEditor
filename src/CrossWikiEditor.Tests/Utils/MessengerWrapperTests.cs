using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Tests.Utils;

public sealed class MessengerWrapperTests
{
    [Test]
    public void Send_ShouldInvokeMessengerSendMethod()
    {
        // Arrange
        IMessenger? mockMessenger = Substitute.For<IMessenger>();
        var wrapper = new MessengerWrapper(mockMessenger);
        var message = new StartBotMessage();
        mockMessenger.Send(message).Returns(message);

        // Act
        StartBotMessage result = wrapper.Send(message);

        // Assert
        mockMessenger.Received(1).Send(message);
        result.Should().Be(message);
    }

    [Test]
    public void Register_ShouldInvokeMessengerRegisterMethod()
    {
        // Arrange
        IMessenger? mockMessenger = Substitute.For<IMessenger>();
        var wrapper = new MessengerWrapper(mockMessenger);
        var recipient = new object();
        MessageHandler<object, StartBotMessage> handler = (sender, args) => { };

        // Act
        wrapper.Register(recipient, handler);

        // Assert
        mockMessenger.Received(1).Register(recipient, handler);
    }
}