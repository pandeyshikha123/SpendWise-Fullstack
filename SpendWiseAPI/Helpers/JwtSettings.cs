namespace SpendWiseAPI.Helpers
{
    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string RefreshSecret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int TokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}
