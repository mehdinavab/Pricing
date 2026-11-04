using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Guarantees.Domain.Entities;

[Table("Vendors", Schema = "guarantees")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(IsDeleted))]
public sealed class Vendor : BaseEntity
{
    private readonly List<Guarantee> _guarantees = [];

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? ContactPerson { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? PhoneNumber { get; private set; }

    public string? Email { get; private set; }

    public string? Address { get; private set; }

    public string? Website { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Guarantee> Guarantees => _guarantees.AsReadOnly();

    private Vendor()
    {
    }

    public Vendor(
        string name,
        string code,
        string? contactPerson = null,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? website = null,
        string? description = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        ContactPerson = contactPerson?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        Website = website?.Trim();
        Description = description?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        string? contactPerson,
        string? phoneNumber,
        string? email,
        string? address,
        string? website,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        ContactPerson = contactPerson?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        Website = website?.Trim();
        Description = description?.Trim();

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

    public void AddGuarantee(Guarantee guarantee)
    {
        ArgumentNullException.ThrowIfNull(guarantee);
        _guarantees.Add(guarantee);
    }
}
