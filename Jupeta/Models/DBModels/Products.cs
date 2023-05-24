using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using MediatR.NotificationPublishers;

namespace Jupeta.Models.DBModels
{
    public class Products
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("productName")]
        public string? ProductName { get; set; } 
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;
        [BsonElement("price")]
        public int Price { get; set; } 
        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; }
        [BsonElement("quantity")]
        public int Quantity { get; set; }
        [BsonElement("addedAt")]
        [BsonDateTimeOptions]
        public DateTime? AddedAt { get; set; }
        [BsonElement("productImage")]
        public string? ProductImage { get; set; }
        [BsonIgnore]
        public IFormFile? ImageFile { get; set; }
    }
}
