using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Users;

namespace MTK.Modules.Buildings.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Bir neçə user ID-si üçün user adlarını (FullName) gətirir.
    /// Tapılmayan ID-lər dictionary-də olmayacaq.
    /// </summary>
    Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
}
