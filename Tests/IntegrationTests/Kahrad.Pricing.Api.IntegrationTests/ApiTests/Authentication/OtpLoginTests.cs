using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Domain.Enums;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Authentication;

public sealed partial class OtpLoginTests(KahradPricingApiWebAppFactory factory)
    : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Otp_request_and_verification_issue_tokens_and_consume_challenge()
    {
        using var client = AnonymousClient();
        using var requestResponse = await client.PostAsJsonAsync(
            "/api/identity/auth/otp/request",
            new RequestOtpRequest { PhoneNumber = "09121234567" });
        Assert.Equal(HttpStatusCode.Accepted, requestResponse.StatusCode);
        var request = await ReadResultAsync<OtpRequestResponse>(requestResponse);

        var messageBody = await ReadDatabaseAsync<SmsDbContext, string?>(db =>
            db.Messages.Where(x => x.Type == SmsType.Otp)
                .Select(x => x.MessageBody)
                .SingleAsync());
        var code = OtpCodeRegex().Match(messageBody ?? string.Empty).Value;
        Assert.Equal(6, code.Length);

        using var verifyResponse = await client.PostAsJsonAsync(
            "/api/identity/auth/otp/verify",
            new VerifyOtpRequest
            {
                RequestId = request.RequestId,
                PhoneNumber = "09121234567",
                Code = code
            });
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        var tokens = await ReadResultAsync<TokenResponseDto>(verifyResponse);
        Assert.NotEmpty(new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken).Claims);

        var consumed = await ReadDatabaseAsync<IdentityDbContext, DateTime?>(db =>
            db.OtpChallenges.Where(x => x.PublicId == request.RequestId)
                .Select(x => x.ConsumedAtUtc)
                .SingleAsync());
        Assert.NotNull(consumed);

        using var replayResponse = await client.PostAsJsonAsync(
            "/api/identity/auth/otp/verify",
            new VerifyOtpRequest
            {
                RequestId = request.RequestId,
                PhoneNumber = "09121234567",
                Code = code
            });
        Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);
    }

    [SqlServerFact]
    public async Task Unknown_phone_gets_generic_response_without_queued_sms_or_challenge()
    {
        using var client = AnonymousClient();
        using var response = await client.PostAsJsonAsync(
            "/api/identity/auth/otp/request",
            new RequestOtpRequest { PhoneNumber = "09120000000" });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var result = await ReadResultAsync<OtpRequestResponse>(response);
        Assert.NotEqual(Guid.Empty, result.RequestId);
        Assert.Equal(0, await ReadDatabaseAsync<IdentityDbContext, int>(db =>
            db.OtpChallenges.CountAsync()));
        Assert.Equal(0, await ReadDatabaseAsync<SmsDbContext, int>(db =>
            db.Messages.CountAsync(x => x.Type == SmsType.Otp)));
    }

    [SqlServerFact]
    public async Task Resend_during_cooldown_does_not_queue_another_code()
    {
        using var client = AnonymousClient();
        var body = new RequestOtpRequest { PhoneNumber = "09121234567" };
        using var first = await client.PostAsJsonAsync("/api/identity/auth/otp/request", body);
        using var second = await client.PostAsJsonAsync("/api/identity/auth/otp/request", body);

        Assert.Equal(HttpStatusCode.Accepted, first.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, second.StatusCode);
        Assert.Equal(1, await ReadDatabaseAsync<SmsDbContext, int>(db =>
            db.Messages.CountAsync(x => x.Type == SmsType.Otp)));
        Assert.Equal(1, await ReadDatabaseAsync<IdentityDbContext, int>(db =>
            db.OtpChallenges.CountAsync()));
    }

    [GeneratedRegex("[0-9]{6}")]
    private static partial Regex OtpCodeRegex();
}
