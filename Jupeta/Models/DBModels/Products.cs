﻿using MongoDB.Bson.Serialization.Attributes;
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
        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;
        [BsonElement("condition")]
        public string? Condition { get; set; }
        public string? SellingType { get; set; }
        [BsonElement("productImage")]
        public Guid ProductImage { get; set; }
        [BsonElement("imageFile")]
        public object? ImageFileUrl { get; set; } = string.Empty;
        [BsonIgnore]
        public FormFile? ImageFile { get; set; }
        [BsonElement("addedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime AddedAt { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

    }

}

//make category required