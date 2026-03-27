using Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventService.Models
{
    public class EventDocument
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid EntityId { get; set; }

        [BsonRepresentation(BsonType.String)] 
        public EntityType EntityType { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public int Version { get; set; } = 1;
    }
}
