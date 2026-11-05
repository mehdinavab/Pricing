using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Services;

public interface ISmsService
{
    Task<SmsBatchResponse> SendBulkAsync(
        SendBulkSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<SmsBatchResponse> SendPeerToPeerAsync(
        SendPeerToPeerSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<SmsBatchResponse> SendTemplateAsync(
        SendTemplateSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<SmsBatchResponse> SendOtpAsync(
        SendOtpSmsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SmsLogDto>> GetLogsAsync(long messageId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    Task<SmsMessageDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<SmsMessageDto>> GetPagedAsync(
        SmsQueryDto filter,
        CancellationToken cancellationToken = default);
}
