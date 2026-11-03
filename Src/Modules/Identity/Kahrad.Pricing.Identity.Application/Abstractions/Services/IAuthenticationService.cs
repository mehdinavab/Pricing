using Kahrad.Pricing.Identity.Application.DTOs.Authentication;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDto> LoginWithPasswordAsync(
            PasswordLoginRequest request,
            CancellationToken cancellationToken = default);

        Task<TokenResponseDto> RefreshTokenAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default);

        Task LogoutAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default);

        Task LogoutAllAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
