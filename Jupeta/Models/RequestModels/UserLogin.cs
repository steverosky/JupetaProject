namespace Jupeta.Models.RequestModels
{
    public class UserLogin
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
