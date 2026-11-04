namespace Kahrad.Pricing.Identity.Application.DTOs.Users
{
    public sealed class UserDto
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public IReadOnlyCollection<string> Roles { get; init; } = [];
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
    }
}
