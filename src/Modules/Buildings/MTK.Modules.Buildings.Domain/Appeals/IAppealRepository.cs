using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Repository for Appeal aggregate.
/// </summary>
public interface IAppealRepository : IRepository<Appeal>
{
    /// <summary>
    /// Appeal-i comment-lərlə birlikdə gətir.
    /// </summary>
    Task<Appeal?> GetWithCommentsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// İstifadəçinin bütün müraciətlərini gətir.
    /// </summary>
    Task<List<Appeal>> GetBySubmitterIdAsync(Guid submitterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verilmiş müraciətlər üzrə şərh saylarını bir sorğu ilə gətir.
    /// </summary>
    Task<Dictionary<Guid, int>> GetCommentCountsAsync(IReadOnlyCollection<Guid> appealIds, CancellationToken cancellationToken = default);
}
