using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Application.Exceptions;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Authentication
{
    public sealed class AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IOtpChallengeRepository otpChallenges,
        IOtpNotificationSender otpNotifications,
        IOptions<OtpAuthenticationOptions> otpOptions,
        TimeProvider timeProvider) : IAuthenticationService
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

        public async Task<OtpRequestResponse> RequestOtpAsync(
            RequestOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            var options = otpOptions.Value;
            options.Validate();
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var genericResponse = new OtpRequestResponse(
                Guid.NewGuid(),
                now.AddMinutes(options.LifetimeMinutes),
                options.ResendCooldownSeconds);

            var normalizedPhone = NormalizeIranianMobile(request.PhoneNumber);
            var phoneCandidates = PhoneCandidates(normalizedPhone);
            var matchingUsers = await userManager.Users
                .Where(x => x.PhoneNumber != null && phoneCandidates.Contains(x.PhoneNumber))
                .Take(2)
                .ToListAsync(cancellationToken);

            if (matchingUsers.Count != 1)
                return genericResponse;

            var user = matchingUsers[0];
            if (!user.IsActive || await userManager.IsLockedOutAsync(user))
                return genericResponse;

            var currentChallenge = await otpChallenges.GetActiveByUserIdAsync(
                user.Id,
                now,
                cancellationToken);
            if (currentChallenge is not null && currentChallenge.NextRequestAtUtc > now)
                return genericResponse;

            var code = GenerateNumericCode(options.CodeLength);
            currentChallenge?.Invalidate(now);
            var challenge = OtpChallenge.Create(
                user.Id,
                code,
                now,
                TimeSpan.FromMinutes(options.LifetimeMinutes),
                TimeSpan.FromSeconds(options.ResendCooldownSeconds),
                options.MaxFailedAttempts);

            await otpChallenges.ReplaceActiveAsync(
                currentChallenge,
                challenge,
                cancellationToken);

            try
            {
                await otpNotifications.SendAsync(
                    normalizedPhone,
                    code,
                    challenge.ExpiresAtUtc,
                    cancellationToken);
            }
            catch
            {
                challenge.Invalidate(timeProvider.GetUtcNow().UtcDateTime);
                await otpChallenges.UpdateAsync(challenge, cancellationToken);
                throw;
            }

            return new OtpRequestResponse(
                challenge.PublicId,
                challenge.ExpiresAtUtc,
                options.ResendCooldownSeconds);
        }

        public async Task<TokenResponseDto> LoginWithOtpAsync(
            VerifyOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var challenge = await otpChallenges.GetByPublicIdAsync(
                request.RequestId,
                cancellationToken);
            if (challenge is null || !challenge.IsUsable(now))
                throw InvalidOtp();

            var user = await userManager.FindByIdAsync(challenge.UserId.ToString());
            if (user is null || !user.IsActive || await userManager.IsLockedOutAsync(user) ||
                !string.Equals(
                    NormalizeIranianMobile(user.PhoneNumber ?? string.Empty),
                    NormalizeIranianMobile(request.PhoneNumber),
                    StringComparison.Ordinal))
            {
                challenge.Invalidate(now);
                await otpChallenges.UpdateAsync(challenge, cancellationToken);
                throw InvalidOtp();
            }

            if (!challenge.Verify(request.Code, now))
            {
                await otpChallenges.UpdateAsync(challenge, cancellationToken);
                EnsureSucceeded(await userManager.AccessFailedAsync(user));
                throw InvalidOtp();
            }

            challenge.Consume(now);
            user.PhoneNumberConfirmed = true;
            EnsureSucceeded(await userManager.UpdateAsync(user));
            EnsureSucceeded(await userManager.ResetAccessFailedCountAsync(user));

            var tokens = await tokenGenerator.GenerateAsync(user);
            var refreshToken = CreateRefreshToken(user.Id, Guid.NewGuid(), tokens);
            await otpChallenges.CompleteAsync(challenge, refreshToken, cancellationToken);
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

        private static string GenerateNumericCode(int length)
        {
            var code = new char[length];
            for (var index = 0; index < code.Length; index++)
                code[index] = (char)('0' + RandomNumberGenerator.GetInt32(10));
            return new string(code);
        }

        private static string NormalizeIranianMobile(string phoneNumber)
        {
            var digits = new string(phoneNumber
                .Where(char.IsAsciiDigit)
                .ToArray());

            if (digits.StartsWith("0098", StringComparison.Ordinal))
                digits = digits[4..];
            else if (digits.StartsWith("98", StringComparison.Ordinal))
                digits = digits[2..];
            else if (digits.StartsWith('0'))
                digits = digits[1..];

            return string.Create(CultureInfo.InvariantCulture, $"0{digits}");
        }

        private static string[] PhoneCandidates(string normalizedPhone)
        {
            var subscriber = normalizedPhone[1..];
            return
            [
                normalizedPhone,
                string.Create(CultureInfo.InvariantCulture, $"+98{subscriber}"),
                string.Create(CultureInfo.InvariantCulture, $"0098{subscriber}"),
                string.Create(CultureInfo.InvariantCulture, $"98{subscriber}")
            ];
        }

        private static UnauthorizedAccessException InvalidOtp() =>
            new("Invalid or expired verification code.");

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
