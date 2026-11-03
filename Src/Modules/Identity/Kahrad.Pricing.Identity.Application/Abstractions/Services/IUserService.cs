using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

        Task<BaseResponsePaginated<UserDto>> GetPagedAsync(UserQueryDto filter, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

        Task SetActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

        Task ResetPasswordAsync(Guid userId, ResetPasswordRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

        Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    }
}
