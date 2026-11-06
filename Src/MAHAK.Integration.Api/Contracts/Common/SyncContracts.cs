namespace MAHAK.Integration.Api.Contracts.Common;

public record SyncRequest
{
    public string? Cursor { get; init; }
    public int? PageSize { get; init; }
}

public sealed record SyncResponse<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore);
