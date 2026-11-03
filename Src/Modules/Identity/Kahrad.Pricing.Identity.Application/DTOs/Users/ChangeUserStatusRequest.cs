namespace Kahrad.Pricing.Identity.Application.DTOs.Users;

public sealed class ChangeUserStatusRequest
{
    public required bool IsActive { get; init; }
}
