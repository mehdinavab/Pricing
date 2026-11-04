namespace Kahrad.Pricing.Brands.Application.Exceptions;

public sealed class StoreNotFoundException : KeyNotFoundException
{
    public StoreNotFoundException(long storeId)
        : base($"Store with ID '{storeId}' was not found.")
    {
    }

    public StoreNotFoundException(Guid publicId)
        : base($"Store with Public ID '{publicId}' was not found.")
    {
    }

    public StoreNotFoundException(long brandId, string code)
        : base($"Store with code '{code}' for Brand '{brandId}' was not found.")
    {
    }
}
