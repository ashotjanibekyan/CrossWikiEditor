using CommunityToolkit.Mvvm.Messaging;

namespace CrossWikiEditor.Utils;

public interface IMessengerWrapper
{
    TMessage Send<TMessage>(TMessage message)
        where TMessage : class;

    void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
        where TMessage : class;
}

public class MessengerWrapper(IMessenger messenger) : IMessengerWrapper
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
}