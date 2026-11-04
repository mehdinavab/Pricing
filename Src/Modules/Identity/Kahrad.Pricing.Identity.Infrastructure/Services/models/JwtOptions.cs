namespace Kahrad.Pricing.Identity.Infrastructure.Services.models
{
    public sealed class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int AccessTokenLifetimeMinutes { get; set; } = 60;
        public int RefreshTokenLifetimeDays { get; set; } = 1;
    }
}
