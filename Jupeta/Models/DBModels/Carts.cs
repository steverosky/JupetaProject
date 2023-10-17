using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Jupeta.Models.DBModels
{
    public class Carts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("userId")]
        public required string UserId { get; set; }
        
        [BsonElement("cartItems")]
        public List<Products>? Products { get; set; }
        [BsonElement("dateAdded")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public required DateTime DateAdded { get; set; }

    }
}