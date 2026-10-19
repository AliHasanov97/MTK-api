using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Domain.Articles;

public interface IArticleRepository : IRepository<Article>
{
    /// <summary>
    /// Son yayımlanan məqalələri gətir (landing page üçün)
    /// </summary>
    Task<List<Article>> GetLatestPublishedAsync(
        int count = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bütün məqalələri siyahıya al (pagination ilə)
    /// </summary>
    Task<(List<Article> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        ArticleStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Məqaləni ID-yə görə gətir və baxış sayını artır
    /// </summary>
    Task<Article?> GetByIdAndIncrementViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
