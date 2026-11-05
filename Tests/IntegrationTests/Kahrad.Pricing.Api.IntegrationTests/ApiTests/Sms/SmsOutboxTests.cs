using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Enums;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Sms;
public sealed class SmsOutboxTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    private async Task<long> QueueAsync()
    {
        using var response=await Client.PostAsJsonAsync("/api/sms/send-bulk",new SendBulkSmsRequest{PhoneNumbers=["09121234567"],Message="Integration message"});
        Assert.Equal(HttpStatusCode.Accepted,response.StatusCode);
        return await ReadDatabaseAsync<SmsDbContext,long>(db=>db.Messages.Select(x=>x.Id).SingleAsync());
    }
    [SqlServerFact]
    public async Task Queue_does_not_send_until_outbox_is_processed()
    {
        var id=await QueueAsync();Assert.Equal(0,Factory.SmsProvider.Calls);
        Assert.Equal(SmsStatus.Pending,await ReadDatabaseAsync<SmsDbContext,SmsStatus>(db=>db.Messages.Where(x=>x.Id==id).Select(x=>x.Status).SingleAsync()));
    }
    [SqlServerFact]
    public async Task Successful_processing_persists_message_receipt_and_log_once()
    {
        var id=await QueueAsync();using var response=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        Assert.Equal(1,(await ReadResultAsync<SmsOutboxProcessResult>(response)).SentCount);Assert.Equal(1,Factory.SmsProvider.Calls);
        var message=await ReadDatabaseAsync<SmsDbContext,Kahrad.Pricing.Sms.Domain.Entities.SmsMessage>(db=>db.Messages.AsNoTracking().SingleAsync(x=>x.Id==id));Assert.Equal(SmsStatus.Sent,message.Status);Assert.Equal("test-receipt",message.ProviderMessageId);Assert.Equal(100m,message.Cost);
        var log=await ReadDatabaseAsync<SmsDbContext,Kahrad.Pricing.Sms.Domain.Entities.SmsLog>(db=>db.Logs.AsNoTracking().SingleAsync(x=>x.SmsMessageId==id));Assert.Equal(SmsLogStatus.Succeeded,log.Status);Assert.NotNull(log.CompletedAtUtc);
        using var repeated=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,repeated.StatusCode);Assert.Equal(1,Factory.SmsProvider.Calls);
        using var logs=await Client.GetAsync($"/api/sms/{id}/logs");Assert.Equal(HttpStatusCode.OK,logs.StatusCode);Assert.Single(await ReadResultAsync<List<SmsLogDto>>(logs));
    }
    [SqlServerFact]
    public async Task Retryable_rejection_is_delayed_and_not_immediately_resent()
    {
        var id=await QueueAsync();Factory.SmsProvider.Result=SmsProviderResult.Failure("Provider busy",canRetry:true);
        using var response=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        var message=await ReadDatabaseAsync<SmsDbContext,Kahrad.Pricing.Sms.Domain.Entities.SmsMessage>(db=>db.Messages.AsNoTracking().SingleAsync(x=>x.Id==id));Assert.Equal(SmsStatus.Pending,message.Status);Assert.Equal(1,message.RetryCount);Assert.True(message.NextAttemptAtUtc>DateTime.UtcNow);
        using var repeated=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,repeated.StatusCode);Assert.Equal(1,Factory.SmsProvider.Calls);
    }
    [SqlServerFact]
    public async Task Unknown_delivery_outcome_is_terminal_and_not_retried()
    {
        var id=await QueueAsync();Factory.SmsProvider.Result=SmsProviderResult.Failure("Outcome unknown",canRetry:true,outcomeUnknown:true);
        using var response=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        Assert.Equal(SmsStatus.Failed,await ReadDatabaseAsync<SmsDbContext,SmsStatus>(db=>db.Messages.Where(x=>x.Id==id).Select(x=>x.Status).SingleAsync()));
        Assert.Equal(SmsLogStatus.Unknown,await ReadDatabaseAsync<SmsDbContext,SmsLogStatus>(db=>db.Logs.Where(x=>x.SmsMessageId==id).Select(x=>x.Status).SingleAsync()));
        using var repeated=await Client.PostAsJsonAsync("/api/sms/process-outbox",new{});Assert.Equal(HttpStatusCode.OK,repeated.StatusCode);Assert.Equal(1,Factory.SmsProvider.Calls);
    }
    [SqlServerFact]
    public async Task Invalid_phone_is_rejected_without_queueing()
    {
        using var response=await Client.PostAsJsonAsync("/api/sms/send-bulk",new SendBulkSmsRequest{PhoneNumbers=["bad-number"],Message="message"});Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);Assert.Equal(0,await ReadDatabaseAsync<SmsDbContext,int>(db=>db.Messages.CountAsync()));
    }
    [SqlServerFact]
    public async Task Anonymous_raw_otp_sending_is_rejected()
    {
        using var anonymous=AnonymousClient();using var response=await anonymous.PostAsJsonAsync("/api/sms/send-otp",new SendOtpSmsRequest{PhoneNumber="09121234567",Code="654321"});Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);Assert.Equal(0,Factory.SmsProvider.Calls);
    }
}
