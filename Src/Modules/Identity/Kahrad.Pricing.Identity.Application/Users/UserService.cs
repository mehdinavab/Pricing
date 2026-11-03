using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Application.Exceptions;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Identity.Application.Users;

public sealed class UserService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IRefreshTokenRepository refreshTokenRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IUserService
{
    public async Task<Guid> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var roleIds = request.RoleIds.Distinct().ToArray();
        foreach (var roleId in roleIds)
        {
            if (await roleManager.FindByIdAsync(roleId.ToString()) is null)
                throw new KeyNotFoundException("Role not found.");
        }

        var user = new ApplicationUser(
            request.Username,
            request.FirstName,
            request.LastName,
            request.Email);
        user.PhoneNumber = request.PhoneNumber;

        var result = await userManager.CreateAsync(user, request.Password);
        EnsureSucceeded(result);

        foreach (var roleId in roleIds)
        {
            await AssignRoleAsync(user.Id, roleId, cancellationToken);
        }

        return user.Id;
    }

    public async Task<UserDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user is null ? null : await MapUserAsync(user);
    }

    public async Task<UserDto?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.NormalizedUserName == username.ToUpperInvariant(),
                cancellationToken);

        return user is null ? null : await MapUserAsync(user);
    }

    public async Task<BaseResponsePaginated<UserDto>> GetPagedAsync(
        UserQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);
        var query = userManager.Users.AsNoTracking();

        var totalCount = await sieveProcessor
            .Apply(filter, query, applyFiltering: true, applySorting: false, applyPagination: false)
            .CountAsync(cancellationToken);

        var users = await sieveProcessor
            .Apply(filter, query, applyFiltering: true, applySorting: true, applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = new List<UserDto>(users.Count);
        foreach (var user in users)
            data.Add(await MapUserAsync(user));

        return BaseResponsePaginated<UserDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task UpdateAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        user.UpdateProfile(
            request.FirstName,
            request.LastName, DateTime.UtcNow);
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;

        var result = await userManager.UpdateAsync(user);
        EnsureSucceeded(result);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        EnsureSucceeded(result);
        await refreshTokenRepository.RevokeAllByUserIdAsync(
            userId,
            DateTime.UtcNow,
            cancellationToken);
    }

    public async Task ResetPasswordAsync(
        Guid userId,
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var result = await userManager.ResetPasswordAsync(
            user,
            token,
            request.NewPassword);

        EnsureSucceeded(result);
        await refreshTokenRepository.RevokeAllByUserIdAsync(
            userId,
            DateTime.UtcNow,
            cancellationToken);
    }

    public async Task AssignRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        var role = await roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new KeyNotFoundException("Role not found.");

        if (await userManager.IsInRoleAsync(user, role.Name!))
            return;

        var result = await userManager.AddToRoleAsync(user, role.Name!);
        EnsureSucceeded(result);
        await refreshTokenRepository.RevokeAllByUserIdAsync(
            userId,
            DateTime.UtcNow,
            cancellationToken);
    }

    public async Task RemoveRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        var role = await roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new KeyNotFoundException("Role not found.");

        if (!await userManager.IsInRoleAsync(user, role.Name!))
            return;

        var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
        EnsureSucceeded(result);
        await refreshTokenRepository.RevokeAllByUserIdAsync(
            userId,
            DateTime.UtcNow,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        var roles = await userManager.GetRolesAsync(user);

        return roles.ToArray();
    }

    public async Task SetActiveStatusAsync(
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId);

        user.SetActiveStatus(isActive);

        var result = await userManager.UpdateAsync(user);
        EnsureSucceeded(result);

        if (!isActive)
        {
            await refreshTokenRepository.RevokeAllByUserIdAsync(
                userId,
                DateTime.UtcNow,
                cancellationToken);
        }
    }

    private async Task<ApplicationUser> FindUserAsync(Guid userId)
    {
        return await userManager.FindByIdAsync(userId.ToString())
            ?? throw new KeyNotFoundException("User not found.");
    }

    private async Task<UserDto> MapUserAsync(ApplicationUser user)
    {
        var result = mapper.Map<UserDto>(user);
        return new UserDto
        {
            Id = result.Id,
            Username = result.Username,
            Email = result.Email,
            PhoneNumber = result.PhoneNumber,
            FirstName = result.FirstName,
            LastName = result.LastName,
            IsActive = result.IsActive,
            Roles = (await userManager.GetRolesAsync(user)).ToArray(),
            CreatedAtUtc = result.CreatedAtUtc,
            UpdatedAtUtc = result.UpdatedAtUtc
        };
    }

    private static void EnsureStableSort(UserQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "Id"
            : $"{filter.Sorts},Id";
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join(
            "; ",
            result.Errors.Select(x => $"{x.Code}: {x.Description}"));

        throw new IdentityOperationException(errors);
    }
}