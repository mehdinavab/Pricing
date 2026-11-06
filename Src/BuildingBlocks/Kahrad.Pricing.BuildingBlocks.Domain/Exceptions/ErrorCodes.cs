namespace Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Stable application error codes shared by every module and host.
/// Values are API contracts and must not be renumbered.
/// </summary>
public enum ErrorCodes
{
    BadRequest = 100,
    Validation = 110,
    Unauthorized = 200,
    Forbidden = 210,
    NotFound = 300,
    Conflict = 400,
    ServiceUnavailable = 500,
    SystemError = 900
}
