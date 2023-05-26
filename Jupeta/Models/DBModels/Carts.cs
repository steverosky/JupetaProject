using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Jupeta.Models.DBModels
{
    public class Carts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("userId")]
        public required string UserId { get; set; }
        [BsonElement("productId")]
        public required string ProductId { get; set; }
        [BsonElement("productName")]
        public required string ProductName { get; set; }
        [BsonElement("price")]
        public required int Price { get; set; }
        [BsonElement("quantity")]
        public required int Quantity { get; set; }
        [BsonElement("productImage")]
        public required string ProductImage { get; set; }
        [BsonElement("dateAdded")]
        public required DateTime DateAdded { get; set; }

    }
}