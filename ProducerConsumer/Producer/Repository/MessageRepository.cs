using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly IDocumentStore _store;

    public MessageRepository(IDocumentStore store)
    {
        _store = store;
    }

    public async Task SaveMessageAsync(ReceivedMessage message)
    {
        using var session = _store.OpenAsyncSession();
        await session.StoreAsync(message);
        await session.SaveChangesAsync();
    }
}