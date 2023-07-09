using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jupeta.Models.DBModels
{
    public class UserReg
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
        [BsonElement("phoneNumber")]
        public long PhoneNumber { get; set; }
        [BsonElement("dateOfBirth")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateOfBirth { get; set; }
        [BsonElement("CreatedOn")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedOn { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        public string GetFullName()
        {
            return $"{this.FirstName}, {this.LastName}";
        }
    }
}