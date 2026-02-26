using Producer.Models;

namespace Producer.Extensions;

public static class MappingExtensions
{
    public static ProcessingMessage ToProcessingMessage(this ReceivedMessage message)
    {
        return new ProcessingMessage
        {
            Id = message.Id,
            DestinationUrl = message.DestinationUrl,
            Payload = message.Payload,
            StartedAt = DateTime.UtcNow
        };
    }
}