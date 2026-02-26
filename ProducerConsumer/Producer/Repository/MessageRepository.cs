using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Repository;

public class MessageRepository(IDocumentStore store) : IMessageRepository
{
    public async Task SaveReceivedMessageAsync(ReceivedMessage message)
    {
        using var session = store.OpenAsyncSession();
        await session.StoreAsync(message);
        await session.SaveChangesAsync();
    }
}