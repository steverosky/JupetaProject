using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Jupeta.Models.RequestModels
{
    public class AddUserModel
    {       
        public required string FirstName { get; set; } 
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Valid Email is Required")]
        [EmailAddress]
        public required string Email { get; set; }
        [PasswordPropertyText]
        public required string Password { get; set; }
        public required int PhoneNumber { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}
