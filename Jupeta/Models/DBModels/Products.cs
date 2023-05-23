using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Jupeta.Models.DBModels
{
    public class Products
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("productName")]
        public string ProductName { get; set; } = string.Empty;
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;
        [BsonElement("price")]
        public string Price { get; set; } = string.Empty;
        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; }
        [BsonElement("addedAt")]
        [BsonDateTimeOptions]
        public DateTime AddedAt { get; set; }
    }
}
