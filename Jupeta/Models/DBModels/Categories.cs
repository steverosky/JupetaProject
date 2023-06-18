using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Jupeta.Models.DBModels
{
    public class Categories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BindNever]
        public string Id { get; set;} = string.Empty;
        [BsonElement("categoryName")]
        public required string Name { get; set;}
    }
}
