using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Jupeta.Models.DBModels
{
    public class UserExtLogins
    {
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        [BsonElement("providerKey")]
        public required string ProviderKey { get; set; }
        [BsonElement("provider")]
        public required string Provider { get; set; }
        [BsonElement("email")]
        public required string Email { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public required DateTime DateAdded { get; set; }

    }
}
