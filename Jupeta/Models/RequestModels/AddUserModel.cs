using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Jupeta.Models.RequestModels
{
    public class AddUserModel
    {       
        public required string FirstName { get; set; } 
        public required string LastName { get; set; }
        [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z.]{2,}$", ErrorMessage = "Valid Email is Required")]
        public required string Email { get; set; }
        [PasswordPropertyText]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*#?&.])[A-Za-z\\d@$!%*#?&.]{10,}$", ErrorMessage = "Password Format Invalid")]
        public required string Password { get; set; }
        public required long PhoneNumber { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}

//[Required(ErrorMessage = "Mobile no. is required")]
//[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
//public string Phone { get; set; }
//^([\+] ? 33[-] ?|[0])?[1 - 9][0 - 9]{ 8}$

//At least one uppercase letter.
//At least one lowercase letter.
//At least one digit.
//At least one special character from the set [@, $, !, %, *, #, ?, &].
//Minimum length of 10 characters.
