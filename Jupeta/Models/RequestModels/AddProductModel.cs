using MongoDB.Bson.Serialization.Attributes;

namespace Jupeta.Models.RequestModels
{
    public class AddProductModel
    {       
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required string Summary { get; set; } 
        public required double Price { get; set; } 
        public required bool IsAvailable { get; set; }
        public required int Quantity { get; set; }
        public required IFormFile ImageFile { get; set; }
    }
}

