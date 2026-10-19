using MTK.Common.Domain.Abstractions;
using MTK.Modules.Communications.Domain.Articles.Events;
using MTK.Modules.Communications.Domain.FileAttachments;
using MTK.Modules.Communications.Domain.Users;

namespace MTK.Modules.Communications.Domain.Articles;

/// <summary>
/// Xəbər/Məqalə entity
/// </summary>
public sealed class Article : SearchableEntity
{
    private Article()
    {
    }

    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public ArticleStatus Status { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; } = null!;
    public int ViewCount { get; private set; }
    public ICollection<FileAttachment> FileAttachments { get; private set; } = new List<FileAttachment>();

    /// <summary>
    /// Yeni məqalə yarat
    /// </summary>
    public static Article Create(
        string title,
        string summary,
        string content,
        Guid createdById)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = title,
            Summary = summary,
            Content = content,
            Status = ArticleStatus.Draft,
            CreatedById = createdById,
            ViewCount = 0
        };

        article.RaiseDomainEvent(new ArticleCreatedDomainEvent(article.Id));

        return article;
    }

    /// <summary>
    /// Məqaləni yayımla
    /// </summary>
    public void Publish()
    {
        if (Status == ArticleStatus.Published)
        {
            return;
        }

        Status = ArticleStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ArticlePublishedDomainEvent(Id));
    }

    /// <summary>
    /// Məqaləni redaktə et
    /// </summary>
    public void Update(
        string title,
        string summary,
        string content)
    {
        Title = title;
        Summary = summary;
        Content = content;
    }

    /// <summary>
    /// Məqaləni arxivləşdir
    /// </summary>
    public void Archive()
    {
        Status = ArticleStatus.Archived;
    }

    /// <summary>
    /// Baxış sayını artır
    /// </summary>
    public void IncrementViewCount()
    {
        ViewCount++;
    }
}