namespace Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;

public class BaseResponse<T>
{
    public bool Succeeded { get; init; }

    public string? Message { get; init; }

    public T? Data { get; init; }

    public IReadOnlyCollection<string> Errors { get; init; } = [];

    public static BaseResponse<T> Success(
        T? data = default,
        string? message = null)
        => new()
        {
            Succeeded = true,
            Data = data,
            Message = message
        };

    public static BaseResponse<T> Failure(
        string message,
        IEnumerable<string>? errors = null)
        => new()
        {
            Succeeded = false,
            Message = message,
            Errors = errors?.ToArray() ?? []
        };
}