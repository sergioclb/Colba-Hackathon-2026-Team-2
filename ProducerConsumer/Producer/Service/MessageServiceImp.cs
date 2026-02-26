using Producer.Models;
using Producer.Repository;

namespace Producer.Service;

public class MessageServiceImp: IMessageService
{
    private readonly IMessageRepository _repository;

    public MessageServiceImp(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReceivedMessage> ProcessMessageAsync(string payload, string destinationUrl)
    {
        var message = new ReceivedMessage
        {
            Id = Guid.NewGuid().ToString(),
            Payload = payload,
            DestinationUrl = destinationUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.SaveMessageAsync(message);

        return message;
    }
}