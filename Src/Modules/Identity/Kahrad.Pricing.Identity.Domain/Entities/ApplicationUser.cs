using Microsoft.AspNetCore.Identity;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("AspNetUsers", Schema = "identity")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string FirstName { get; private set; } = null!;
        [Sieve(CanFilter = true, CanSort = true)]
        public string LastName { get; private set; } = null!;
        [Sieve(CanFilter = true, CanSort = true)]
        public bool IsActive { get; private set; } = true;

        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; private set; }

        private ApplicationUser()
        {
        }

        public ApplicationUser(
            string userName,
            string firstName,
            string lastName,
            string? email = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userName);
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

            Id = Guid.NewGuid();
            UserName = userName.Trim();
            NormalizedUserName = UserName.ToUpperInvariant();

            FirstName = firstName.Trim();
            LastName = lastName.Trim();

            Email = email?.Trim();
            NormalizedEmail = Email?.ToUpperInvariant();

            SecurityStamp = Guid.NewGuid().ToString();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        public void UpdateProfile(
            string firstName,
            string lastName,
            DateTime utcNow)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            UpdatedAtUtc = utcNow;
        }

        public void Activate()
        {
            IsActive = true;
        }
        public void SetActiveStatus(bool isActive)
        {
            if (IsActive == isActive)
                return;

            IsActive = isActive;
            UpdatedAtUtc = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
