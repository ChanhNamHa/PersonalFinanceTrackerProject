namespace PersonalFinanceTracker.Application.Settings
{
    public class JwtOptions
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        // Access token lifetime in minutes
        public int AccessTokenExpirationMinutes { get; set; } = 60;
        // Refresh token lifetime in days
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
