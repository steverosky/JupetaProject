namespace Jupeta.Models.RequestModels
{
    public class RefreshTokenRequest
    {
        public required string  JwtToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
