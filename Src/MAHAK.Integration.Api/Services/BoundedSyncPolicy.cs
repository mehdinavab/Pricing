using MAHAK.Integration.Api.Configuration;
using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Errors;
using Microsoft.Extensions.Options;

namespace MAHAK.Integration.Api.Services;

public sealed class BoundedSyncPolicy(IOptions<MahakOptions> options)
{
    public int Validate(SyncRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            throw new SchemaConfirmationRequiredException(
                "Cursor pagination is unavailable until a unique, stable MAHAK ordering key is confirmed.");
        }

        var pageSize = request.PageSize ?? options.Value.DefaultPageSize;
        if (pageSize < 1 || pageSize > options.Value.MaximumPageSize)
        {
            throw new RequestValidationException(
                $"PageSize must be between 1 and {options.Value.MaximumPageSize}.");
        }
        return pageSize;
    }

    public static SyncResponse<T> Complete<T>(IReadOnlyList<T> rows, int pageSize)
    {
        if (rows.Count > pageSize)
        {
            throw new SchemaConfirmationRequiredException(
                "The result exceeds one bounded page. A verified unique MAHAK ordering key is required before continuation can be exposed safely.");
        }
        return new SyncResponse<T>(rows, null, false);
    }
}
