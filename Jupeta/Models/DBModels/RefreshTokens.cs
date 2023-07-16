namespace Jupeta.Models.DBModels
{
    public class RefreshTokens
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string RToken { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set;}
    }
}
