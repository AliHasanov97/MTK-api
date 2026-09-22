using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IdentityDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("UserRepository.Add() called for user: {Email}, Id: {UserId}", user.Email, user.Id);
        _logger.LogInformation("DEBUG: UserRepository._context instance = {HashCode}, Type = {Type}",
            _context.GetHashCode(), _context.GetType().FullName);

        _context.Users.Add(user);

        var entry = _context.Entry(user);
        _logger.LogInformation("After Add() - Entity State: {State}", entry.State);

        var trackedCount = _context.ChangeTracker.Entries<User>().Count();
        _logger.LogInformation("ChangeTracker has {Count} User entities", trackedCount);
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
}
