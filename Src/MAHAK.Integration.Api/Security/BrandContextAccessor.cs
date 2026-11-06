namespace MAHAK.Integration.Api.Security;

public interface IBrandContextAccessor
{
    BrandContext GetRequired(HttpContext context);
}

public sealed class BrandContextAccessor : IBrandContextAccessor
{
    public BrandContext GetRequired(HttpContext context) =>
        context.Items.TryGetValue(ApiKeyAuthenticationHandler.BrandContextItemKey, out var value) &&
        value is BrandContext brand
            ? brand
            : throw new InvalidOperationException("Authenticated brand context is unavailable.");
}
