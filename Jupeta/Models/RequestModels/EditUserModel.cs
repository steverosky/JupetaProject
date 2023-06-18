using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Jupeta.Models.RequestModels
{
    public class EditUserModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Valid Email is Required")]
        [EmailAddress]
        public required string Email { get; set; }
        public long? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
