using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Owners;

namespace MTK.Modules.Buildings.Domain.Repositories;

/// <summary>
/// Owner repository interface
/// </summary>
public interface IOwnerRepository : IRepository<Owner>
{
    Task<Owner?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Owner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Owner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
