using Contracts;
using EventService.Models;
using MongoDB.Driver;

namespace EventService.Repos
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<EventDocument> collection;

        public EventRepository(IMongoClient client, IConfiguration configuration)
        {
            var db = client.GetDatabase(configuration["MongoDB:Database"]);
            collection = db.GetCollection<EventDocument>("events");
        }

        public void EnsureIndexes()
        {
            var keys = Builders<EventDocument>.IndexKeys;
            collection.Indexes.CreateMany(
            [
                new CreateIndexModel<EventDocument>(keys.Ascending(z => z.EntityId)),
                new CreateIndexModel<EventDocument>(keys.Ascending(z => z.EntityType)),
                new CreateIndexModel<EventDocument>(keys.Ascending(z => z.Timestamp))
            ]);
        }

        public async Task DeleteOlderByCutoffDate(DateTime date, CancellationToken ct = default)
        {
            var filter = Builders<EventDocument>.Filter.Lt(e => e.Timestamp, date);
            await collection.DeleteManyAsync(filter, ct);
        }

        public async Task<(IEnumerable<EventDocument> Items, long Total)> GetByEntityIdAsync(Guid entityId, int page, int pageSize, CancellationToken ct = default)
        {
            var filter = Builders<EventDocument>.Filter.Eq(z => z.EntityId, entityId);
            return await FilterAndReturn(page, pageSize, filter, ct);
        }

        public async Task<(IEnumerable<EventDocument> Items, long Total)> GetByEntityTypeAsync(EntityType entityType, int page, int pageSize, CancellationToken ct = default)
        {
            var filter = Builders<EventDocument>.Filter.Eq(e => e.EntityType, entityType);
            return await FilterAndReturn(page, pageSize, filter, ct);
        }

        public Task InsertAsync(EventDocument doc, CancellationToken ct = default)
        {
            return collection.InsertOneAsync(doc, cancellationToken: ct);
        }

        private async Task<(IEnumerable<EventDocument> Items, long Total)> FilterAndReturn(int page, int pageSize, FilterDefinition<EventDocument> filter, CancellationToken ct)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
            if (pageSize < 1 || pageSize > 1000)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be between 1 and 1000.");

            var total = await collection.CountDocumentsAsync(filter, cancellationToken: ct);
            var items = await collection.Find(filter)
                .SortByDescending(e => e.Timestamp)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
