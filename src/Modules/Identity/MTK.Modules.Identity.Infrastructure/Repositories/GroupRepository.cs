using Microsoft.EntityFrameworkCore;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

internal sealed class GroupRepository : IGroupRepository
{
    private readonly IdentityDbContext _context;

    public GroupRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Group?> GetByKeycloakGroupIdAsync(Guid keycloakGroupId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(g => g.KeycloakGroupId == keycloakGroupId, cancellationToken);
    }

    public async Task<IReadOnlyList<Group>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .ToListAsync(cancellationToken);
    }

    public void Add(Group group)
    {
        _context.Groups.Add(group);
    }

    public void Update(Group group)
    {
        _context.Groups.Update(group);
    }

    public void Remove(Group group)
    {
        group.Delete();
    }

    public async Task AddUserToGroupAsync(Guid groupId, Guid userId, Guid? assignedBy, CancellationToken cancellationToken = default)
    {
        var userGroup = UserGroup.Create(userId, groupId, assignedBy);
        await _context.UserGroups.AddAsync(userGroup, cancellationToken);
    }

    public async Task RemoveUserFromGroupAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        var userGroup = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId, cancellationToken);

        if (userGroup != null)
        {
            _context.UserGroups.Remove(userGroup);
        }
    }

    public async Task<IReadOnlyList<User>> GetGroupMembersAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.UserGroups
            .Where(ug => ug.GroupId == groupId)
            .Join(_context.Users,
                ug => ug.UserId,
                u => u.Id,
                (ug, u) => u)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignRolesToGroupAsync(Guid groupId, IEnumerable<Guid> roleIds, Guid? assignedBy, CancellationToken cancellationToken = default)
    {
        var groupRoles = roleIds.Select(roleId => GroupRole.Create(groupId, roleId, assignedBy));
        await _context.GroupRoles.AddRangeAsync(groupRoles, cancellationToken);
    }

    public async Task RemoveRolesFromGroupAsync(Guid groupId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var groupRoles = await _context.GroupRoles
            .Where(gr => gr.GroupId == groupId && roleIds.Contains(gr.RoleId))
            .ToListAsync(cancellationToken);

        _context.GroupRoles.RemoveRange(groupRoles);
    }

    public async Task<IReadOnlyList<Role>> GetGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupRoles
            .Where(gr => gr.GroupId == groupId)
            .Join(_context.Roles,
                gr => gr.RoleId,
                r => r.Id,
                (gr, r) => r)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Group>> GetSubGroupsAsync(Guid parentGroupId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.ParentGroupId == parentGroupId)
            .ToListAsync(cancellationToken);
    }
}
