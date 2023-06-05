using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Jupeta.Models.RequestModels
{
    public class UserLogin
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [PasswordPropertyText]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
