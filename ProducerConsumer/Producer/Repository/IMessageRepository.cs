using Producer.Models;

namespace Producer.Repository;

public interface IMessageRepository
{
    Task SaveMessageAsync(ReceivedMessage message);
}