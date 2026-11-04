using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Brands.Domain.Entities;

[Table("Brands", Schema = "brands")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(IsDeleted))]
public sealed class Brand : BaseEntity
{
    private readonly List<BrandConfig> _configs = [];
    private readonly List<Store> _stores = [];

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    public string? LogoUrl { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    [Sieve(CanFilter = true, CanSort = true)]
    public int ActivePriceVersion { get; private set; } = 1;

    public string? Website { get; private set; }

    public string? SupportPhone { get; private set; }

    public string? SupportEmail { get; private set; }

    public IReadOnlyCollection<BrandConfig> Configs => _configs.AsReadOnly();

    public IReadOnlyCollection<Store> Stores => _stores.AsReadOnly();

    private Brand()
    {
    }

    public Brand(
        string name,
        string code,
        string? description = null,
        string? logoUrl = null,
        string? website = null,
        string? supportPhone = null,
        string? supportEmail = null,
        int initialActivePriceVersion = 1,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (initialActivePriceVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialActivePriceVersion), "Version must be positive.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
        LogoUrl = logoUrl?.Trim();
        Website = website?.Trim();
        SupportPhone = supportPhone?.Trim();
        SupportEmail = supportEmail?.Trim();
        ActivePriceVersion = initialActivePriceVersion;
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        string? description,
        string? logoUrl,
        string? website,
        string? supportPhone,
        string? supportEmail,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
        LogoUrl = logoUrl?.Trim();
        Website = website?.Trim();
        SupportPhone = supportPhone?.Trim();
        SupportEmail = supportEmail?.Trim();

        SetUpdated(userId, utcNow);
    }

    public void SetActivePriceVersion(int version, DateTime utcNow, Guid? userId = null)
    {
        if (version <= 0)
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be positive.");

        ActivePriceVersion = version;
        SetUpdated(userId, utcNow);
    }

    public void Activate(DateTime utcNow, Guid? userId = null)
    {
        IsActive = true;
        SetUpdated(userId, utcNow);
    }

    public void Deactivate(DateTime utcNow, Guid? userId = null)
    {
        IsActive = false;
        SetUpdated(userId, utcNow);
    }

    public void SetLogo(string? logoUrl, DateTime utcNow, Guid? userId = null)
    {
        LogoUrl = logoUrl?.Trim();
        SetUpdated(userId, utcNow);
    }

    public void AddConfig(BrandConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        _configs.Add(config);
    }

    public void AddStore(Store store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _stores.Add(store);
    }
}
