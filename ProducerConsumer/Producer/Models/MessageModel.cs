namespace Producer.Models
{
    public class ReceivedMessage
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string DestinationUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProcessingMessage
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string DestinationUrl { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProcessedMessage
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string DestinationUrl { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public class ErrorMessage
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string DestinationUrl { get; set; }
        public DateTime FailedAt { get; set; } = DateTime.UtcNow;
    }
}