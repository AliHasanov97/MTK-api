using Microsoft.EntityFrameworkCore;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _context;

    public RoleRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetByTypeAsync(RoleType roleType, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.RoleType == roleType && r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public void Add(Role role)
    {
        _context.Roles.Add(role);
    }

    public void Update(Role role)
    {
        _context.Roles.Update(role);
    }

    public void Remove(Role role)
    {
        role.Delete();
    }

    public async Task<IReadOnlyList<Group>> GetGroupsWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupRoles
            .Where(gr => gr.RoleId == roleId)
            .Join(_context.Groups,
                gr => gr.GroupId,
                g => g.Id,
                (gr, g) => g)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetUsersWithDirectRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoleAssignments
            .Where(ura => ura.RoleId == roleId)
            .Join(_context.Users,
                ura => ura.UserId,
                u => u.Id,
                (ura, u) => u)
            .ToListAsync(cancellationToken);
    }
}
