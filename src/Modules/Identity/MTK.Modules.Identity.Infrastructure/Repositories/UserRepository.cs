using Microsoft.EntityFrameworkCore;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Remove(User user)
    {
        // Soft delete
        user.Delete();
    }

    // User-Role assignments
    public async Task AssignRolesToUserAsync(Guid userId, IEnumerable<Guid> roleIds, Guid? assignedBy, CancellationToken cancellationToken = default)
    {
        var assignments = roleIds.Select(roleId => UserRoleAssignment.Create(userId, roleId, assignedBy));
        await _context.UserRoleAssignments.AddRangeAsync(assignments, cancellationToken);
    }

    public async Task RemoveRolesFromUserAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.UserRoleAssignments
            .Where(ura => ura.UserId == userId && roleIds.Contains(ura.RoleId))
            .ToListAsync(cancellationToken);

        _context.UserRoleAssignments.RemoveRange(assignments);
    }

    public async Task<IReadOnlyList<Role>> GetUserDirectRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoleAssignments
            .Where(ura => ura.UserId == userId)
            .Join(_context.Roles, ura => ura.RoleId, r => r.Id, (ura, r) => r)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetUserEffectiveRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Direct roles
        var directRoles = await GetUserDirectRolesAsync(userId, cancellationToken);

        // Inherited roles through groups
        var inheritedRoles = await _context.UserGroups
            .Where(ug => ug.UserId == userId)
            .Join(_context.GroupRoles, ug => ug.GroupId, gr => gr.GroupId, (ug, gr) => gr.RoleId)
            .Join(_context.Roles, roleId => roleId, r => r.Id, (roleId, r) => r)
            .ToListAsync(cancellationToken);

        // Combine and remove duplicates
        return directRoles.Concat(inheritedRoles).DistinctBy(r => r.Id).ToList();
    }

    public async Task<IReadOnlyList<Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserGroups
            .Where(ug => ug.UserId == userId)
            .Join(_context.Groups, ug => ug.GroupId, g => g.Id, (ug, g) => g)
            .ToListAsync(cancellationToken);
    }
}
