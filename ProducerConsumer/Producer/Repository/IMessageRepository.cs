using Producer.Models;

namespace Producer.Repository;

public interface IMessageRepository
{
    Task SaveReceivedMessageAsync(ReceivedMessage message);
}