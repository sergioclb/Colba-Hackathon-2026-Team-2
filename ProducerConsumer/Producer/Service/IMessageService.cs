using Producer.Models;

namespace Producer.Service;

public interface IMessageService
{
    Task<ReceivedMessage> ProcessMessageAsync(string payload, string destinationUrl);
}