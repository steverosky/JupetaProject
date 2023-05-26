namespace Jupeta.Models.ResponseModels
{
    public class TokenResponse
    {
        public string Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}