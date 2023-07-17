namespace Jupeta.Models.ResponseModels
{
    public class TokenResponse
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public long PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}