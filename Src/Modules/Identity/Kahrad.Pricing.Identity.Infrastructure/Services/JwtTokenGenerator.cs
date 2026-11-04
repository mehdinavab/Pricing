using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Domain.Entities;
using Kahrad.Pricing.Identity.Infrastructure.Services.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Kahrad.Pricing.Identity.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public JwtTokenGenerator(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<TokenResponseDto> GenerateAsync(ApplicationUser user)
        {
            if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
            {
                throw new InvalidOperationException("Jwt:Key must be configured.");
            }

            DateTime now = DateTime.UtcNow;
            DateTime expiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            ];

            IList<string> roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            SigningCredentials signingCredentials = new(signingKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials);
            var refreshToken = Convert.ToBase64String(
    RandomNumberGenerator.GetBytes(64));

            var refreshTokenExpiresAtUtc = now.AddDays(
                _jwtOptions.RefreshTokenLifetimeDays);

            return new TokenResponseDto()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiresAtUtc = expiresAtUtc,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            };
        }
    }
}
