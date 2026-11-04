using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Application.Exceptions;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Authentication
{
    public sealed class AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenRepository refreshTokenRepository) : IAuthenticationService
    {
        public async Task<TokenResponseDto> LoginWithPasswordAsync(
            PasswordLoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user is null ||
                !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (await userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                EnsureSucceeded(await userManager.AccessFailedAsync(user));
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            EnsureSucceeded(await userManager.ResetAccessFailedCountAsync(user));

            var tokens = await tokenGenerator.GenerateAsync(user);
            var familyId = Guid.NewGuid();

            await refreshTokenRepository.AddAsync(
                CreateRefreshToken(user.Id, familyId, tokens),
                cancellationToken);

            return tokens;
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
        {
            var currentToken = await refreshTokenRepository.GetByTokenHashAsync(
                HashToken(request.RefreshToken),
                cancellationToken);

            if (currentToken is null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (currentToken.IsRevoked)
            {
                await refreshTokenRepository.RevokeFamilyAsync(
                    currentToken.FamilyId,
                    DateTime.UtcNow,
                    cancellationToken);

                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var now = DateTime.UtcNow;
            if (currentToken.IsExpired(now))
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var user = await userManager.FindByIdAsync(currentToken.UserId.ToString());
            if (user is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var tokens = await tokenGenerator.GenerateAsync(user);
            var replacementToken = CreateRefreshToken(
                user.Id,
                currentToken.FamilyId,
                tokens);

            await refreshTokenRepository.ReplaceAsync(
                currentToken,
                replacementToken,
                now,
                cancellationToken);

            return tokens;
        }

        public async Task LogoutAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
        {
            var refreshToken = await refreshTokenRepository.GetByTokenHashAsync(
                HashToken(request.RefreshToken),
                cancellationToken);

            if (refreshToken is null || refreshToken.IsRevoked)
                return;

            refreshToken.Revoke(DateTime.UtcNow);
            await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
        }

        public Task LogoutAllAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return refreshTokenRepository.RevokeAllByUserIdAsync(
                userId,
                DateTime.UtcNow,
                cancellationToken);
        }

        private static RefreshToken CreateRefreshToken(
            Guid userId,
            Guid familyId,
            TokenResponseDto tokens)
        {
            return new RefreshToken(
                userId,
                HashToken(tokens.RefreshToken),
                familyId,
                tokens.RefreshTokenExpiresAtUtc);
        }

        private static string HashToken(string token)
        {
            return Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        private static void EnsureSucceeded(IdentityResult result)
        {
            if (result.Succeeded)
                return;

            throw new IdentityOperationException(string.Join(
                "; ",
                result.Errors.Select(x => $"{x.Code}: {x.Description}")));
        }
    }
}
