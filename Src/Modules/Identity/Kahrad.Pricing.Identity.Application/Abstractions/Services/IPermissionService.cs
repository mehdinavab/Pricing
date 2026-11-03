using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Services
{
    public interface IPermissionService
    {
        Task<PermissionDto?> GetByIdAsync(
            long permissionId,
            CancellationToken cancellationToken = default);

        Task<PermissionDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PermissionDto>> GetByRoleAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PermissionDto>> GetByUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> UserHasPermissionAsync(
            Guid userId,
            string permissionCode,
            CancellationToken cancellationToken = default);
    }
}
