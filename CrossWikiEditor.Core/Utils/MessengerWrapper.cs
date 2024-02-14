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

public sealed class MessengerWrapper(IMessenger messenger) : IMessengerWrapper
{
    public TMessage Send<TMessage>(TMessage message)
        where TMessage : class
    {
        return messenger.Send(message);
    }

    public void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
        where TMessage : class
    {
        messenger.Register(recipient, handler);
    }

    public void Unregister<TMessage>(object recipient)
        where TMessage : class
    {
        messenger.Unregister<TMessage>(recipient);
    }
}