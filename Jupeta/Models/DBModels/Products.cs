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
        public string ProductName { get; set; } = string.Empty;
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;
        [BsonElement("price")]
        public double Price { get; set; }
        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; } = true;
        [BsonElement("quantity")]
        public int Quantity { get; set; }
        [BsonElement("addedAt")]
        [BsonDateTimeOptions]
        public DateTime AddedAt { get; set; }
        [BsonElement("productImage")]
        public string ProductImage { get; set; } = string.Empty;
        [BsonElement("imageFile")]
        public string ImageFileUrl { get; set; } = string.Empty;
        [BsonIgnore]
        public IFormFile? ImageFile { get; set; }
    }
}
