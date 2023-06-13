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
        public required long? PhoneNumber { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}

//[Required(ErrorMessage = "Mobile no. is required")]
//[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
//public string Phone { get; set; }
//^([\+] ? 33[-] ?|[0])?[1 - 9][0 - 9]{ 8}$
