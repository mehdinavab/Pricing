namespace Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;

public class BaseResponsePaginated<T>
{
    public bool Succeeded { get; init; }

    public string? Message { get; init; }

    public IReadOnlyCollection<T> Data { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalCount / PageSize)
        : 0;

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;

    public IReadOnlyCollection<string> Errors { get; init; } = [];

    public static BaseResponsePaginated<T> Success(
        IReadOnlyCollection<T> data,
        int page,
        int pageSize,
        int totalCount,
        string? message = null)
        => new()
        {
            Succeeded = true,
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Message = message
        };

    public static BaseResponsePaginated<T> Failure(
        string message,
        IEnumerable<string>? errors = null)
        => new()
        {
            Succeeded = false,
            Message = message,
            Errors = errors?.ToArray() ?? []
        };
}