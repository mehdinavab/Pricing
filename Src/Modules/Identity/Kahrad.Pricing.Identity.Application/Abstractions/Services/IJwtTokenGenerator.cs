using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Domain.Entities;

public interface IJwtTokenGenerator
{
    Task<TokenResponseDto> GenerateAsync(ApplicationUser user);
}
