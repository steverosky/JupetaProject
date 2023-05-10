using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jupeta.Models
{
    public class Theaters
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("movie_id")]
        public int Movie_id { get; set; }
        [BsonElement("text")]
        public string Text { get; set; } = string.Empty;
        [BsonElement("date")]
        public DateTime? Date { get; set; }
    }
}
