using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System.Text.Json;

namespace Kahrad.Pricing.Sms.Application.Services;

public sealed class SmsService(
    ISmsMessageRepository smsMessageRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor,
    ISmsConfigurationRepository configurations) : ISmsService
{
    public async Task<SmsBatchResponse> SendBulkAsync(
        SendBulkSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var provider = await GetActiveProviderAsync(cancellationToken);
        var batchId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;
        var sendDateUtc = request.SendDateUtc.HasValue && request.SendDateUtc.Value > utcNow
            ? request.SendDateUtc.Value
            : utcNow;

        var messages = request.PhoneNumbers.Select(phone =>
        {
            var msg = SmsMessage.CreateBulk(
                batchId,
                phone,
                request.Message,
                sendDateUtc,
                request.LineNumber);
            msg.AssignProvider(provider);
            msg.SetCreated(currentUserId, utcNow);
            return msg;
        }).ToList();

        await smsMessageRepository.AddRangeAsync(messages, cancellationToken);

        return new SmsBatchResponse
        {
            BatchId = batchId,
            TotalQueued = messages.Count,
            ScheduledSendDateUtc = sendDateUtc,
            Status = "Pending"
        };
    }

    public async Task<SmsBatchResponse> SendPeerToPeerAsync(
        SendPeerToPeerSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var provider = await GetActiveProviderAsync(cancellationToken);
        var batchId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;
        var sendDateUtc = request.SendDateUtc.HasValue && request.SendDateUtc.Value > utcNow
            ? request.SendDateUtc.Value
            : utcNow;

        var messages = request.Items.Select(item =>
        {
            var msg = SmsMessage.CreatePeerToPeer(
                batchId,
                item.PhoneNumber,
                item.Message,
                sendDateUtc,
                request.LineNumber);
            msg.AssignProvider(provider);
            msg.SetCreated(currentUserId, utcNow);
            return msg;
        }).ToList();

        await smsMessageRepository.AddRangeAsync(messages, cancellationToken);

        return new SmsBatchResponse
        {
            BatchId = batchId,
            TotalQueued = messages.Count,
            ScheduledSendDateUtc = sendDateUtc,
            Status = "Pending"
        };
    }

    public async Task<SmsBatchResponse> SendTemplateAsync(
        SendTemplateSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var provider = await GetActiveProviderAsync(cancellationToken);
        var batchId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;
        var sendDateUtc = request.SendDateUtc.HasValue && request.SendDateUtc.Value > utcNow
            ? request.SendDateUtc.Value
            : utcNow;

        var parametersJson = request.Parameters.Count > 0
            ? JsonSerializer.Serialize(request.Parameters)
            : null;

        var message = SmsMessage.CreateTemplate(
            batchId,
            request.PhoneNumber,
            request.TemplateId,
            parametersJson,
            sendDateUtc,
            request.LineNumber);

        message.AssignProvider(provider);
        message.SetCreated(currentUserId, utcNow);

        await smsMessageRepository.AddAsync(message, cancellationToken);

        return new SmsBatchResponse
        {
            BatchId = batchId,
            TotalQueued = 1,
            ScheduledSendDateUtc = sendDateUtc,
            Status = "Pending"
        };
    }

    public async Task<SmsBatchResponse> SendOtpAsync(
        SendOtpSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var provider = await GetActiveProviderAsync(cancellationToken);
        var batchId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;

        var parameters = request.Parameters is null ? new Dictionary<string, string>() : new Dictionary<string, string>(request.Parameters);
        if (!parameters.ContainsKey("Code"))
        {
            parameters["Code"] = request.Code;
        }

        var parametersJson = JsonSerializer.Serialize(parameters);

        var message = SmsMessage.CreateOtp(
            batchId,
            request.PhoneNumber,
            request.Code,
            utcNow,
            request.ExpiresAtUtc,
            request.TemplateId,
            parametersJson,
            request.LineNumber);

        message.AssignProvider(provider);
        message.SetCreated(currentUserId, utcNow);

        await smsMessageRepository.AddAsync(message, cancellationToken);

        return new SmsBatchResponse
        {
            BatchId = batchId,
            TotalQueued = 1,
            ScheduledSendDateUtc = utcNow,
            Status = "Pending"
        };
    }

    public async Task<SmsMessageDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var message = await smsMessageRepository.GetByIdAsync(id, cancellationToken);
        return message is null ? null : mapper.Map<SmsMessageDto>(message);
    }

    public async Task<BaseResponsePaginated<SmsMessageDto>> GetPagedAsync(
        SmsQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = smsMessageRepository.GetQueryable().AsNoTracking();

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        if (filter.Type.HasValue)
            query = query.Where(x => x.Type == filter.Type.Value);

        if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
        {
            var phone = filter.PhoneNumber.Trim();
            query = query.Where(x => x.PhoneNumber.Contains(phone));
        }

        if (filter.BatchId.HasValue)
            query = query.Where(x => x.BatchId == filter.BatchId.Value);

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var messages = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<SmsMessageDto>>(messages);

        return BaseResponsePaginated<SmsMessageDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<IReadOnlyCollection<SmsLogDto>> GetLogsAsync(long messageId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new Kahrad.Pricing.Sms.Application.Exceptions.SmsOperationException("Invalid SMS log pagination.");
        if (await smsMessageRepository.GetByIdAsync(messageId, cancellationToken) is null)
            throw new Kahrad.Pricing.Sms.Application.Exceptions.SmsNotFoundException(messageId);
        var logs = await smsMessageRepository.GetLogsAsync(messageId, page, pageSize, cancellationToken);
        return mapper.Map<List<SmsLogDto>>(logs);
    }

    private async Task<SmsProviderConfiguration> GetActiveProviderAsync(CancellationToken cancellationToken)
    {
        var module = await configurations.GetModuleAsync(cancellationToken);
        if (!module.IsEnabled) throw new Kahrad.Pricing.Sms.Application.Exceptions.SmsOperationException("SMS module is disabled.");
        var provider = await configurations.GetProviderAsync(module.ActiveProviderConfigurationId, cancellationToken);
        if (!provider.IsEnabled) throw new Kahrad.Pricing.Sms.Application.Exceptions.SmsOperationException("Active SMS provider is disabled.");
        return provider;
    }

    private static void EnsureStableSort(SmsQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "-CreatedAtUtc,Id"
            : $"{filter.Sorts},Id";
    }
}
