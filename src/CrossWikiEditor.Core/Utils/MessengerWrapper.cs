using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Core.Utils;

public interface IMessengerWrapper
{
    TMessage Send<TMessage>(TMessage message)
        where TMessage : class;

    void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
        where TMessage : class;

    void Unregister<TMessage>(object recipient)
        where TMessage : class;
}

public sealed class MessengerWrapper : IMessengerWrapper
{
    private readonly IMessenger _messenger;

    public MessengerWrapper(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public TMessage Send<TMessage>(TMessage message)
        where TMessage : class
    {
        return _messenger.Send(message);
    }

    public void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
        where TMessage : class
    {
        _messenger.Register(recipient, handler);
    }

    public void Unregister<TMessage>(object recipient)
        where TMessage : class
    {
        _messenger.Unregister<TMessage>(recipient);
    }
}