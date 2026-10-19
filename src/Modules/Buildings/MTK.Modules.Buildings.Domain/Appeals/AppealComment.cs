using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Müraciətə aid şərh - istifadəçi və admin arasında kommunikasiya.
/// </summary>
public sealed class AppealComment : Entity
{
    private AppealComment() : base() { }

    /// <summary>Hansı müraciətə aiddir.</summary>
    public Guid AppealId { get; private set; }

    /// <summary>Şərhi yazan istifadəçi.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Şərhin mətni.</summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>
    /// Yeni şərh yarat.
    /// </summary>
    internal static AppealComment Create(Guid appealId, string text, Guid userId)
    {
        var comment = new AppealComment
        {
            AppealId = appealId,
            Text = text,
            UserId = userId
        };

        comment.SetCreatedAt();
        return comment;
    }
}
