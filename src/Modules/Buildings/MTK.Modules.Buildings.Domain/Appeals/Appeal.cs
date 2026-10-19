using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Appeals.Events;

namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Müraciət (Appeal) - istifadəçilərin şikayət, təklif və sualları.
/// </summary>
public sealed class Appeal : Entity
{
    private readonly List<AppealComment> _comments = [];

    private Appeal() : base() { }

    /// <summary>Müraciətin başlığı.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Müraciətin təsviri (ətraflı izahat).</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Kateqoriya.</summary>
    public AppealCategory Category { get; private set; }

    /// <summary>Status.</summary>
    public AppealStatus Status { get; private set; }

    /// <summary>Prioritet.</summary>
    public AppealPriority Priority { get; private set; }

    /// <summary>Müraciəti yaradan istifadəçi (Owner/Resident).</summary>
    public Guid SubmitterId { get; private set; }

    /// <summary>Müraciət hansı mənzillə bağlıdır (optional).</summary>
    public Guid? ApartmentId { get; private set; }

    /// <summary>Müraciət hansı qarajla bağlıdır (optional).</summary>
    public Guid? GarageId { get; private set; }

    /// <summary>Həll olunma tarixi.</summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>Bağlanma tarixi.</summary>
    public DateTimeOffset? ClosedAt { get; private set; }

    /// <summary>Müraciətə aid şərhlər.</summary>
    public IReadOnlyCollection<AppealComment> Comments => _comments.AsReadOnly();

    /// <summary>
    /// Yeni müraciət yarat (istifadəçi tərəfindən).
    /// </summary>
    public static Appeal Create(
        string title,
        string description,
        AppealCategory category,
        Guid submitterId,
        Guid? apartmentId = null,
        Guid? garageId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Müraciət başlığı boş ola bilməz", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Müraciət təsviri boş ola bilməz", nameof(description));

        if (title.Length > 200)
            throw new ArgumentException("Müraciət başlığı 200 simvoldan çox ola bilməz", nameof(title));

        if (description.Length > 2000)
            throw new ArgumentException("Müraciət təsviri 2000 simvoldan çox ola bilməz", nameof(description));

        var appeal = new Appeal
        {
            Title = title.Trim(),
            Description = description.Trim(),
            Category = category,
            Status = AppealStatus.Submitted,
            Priority = AppealPriority.Normal,
            SubmitterId = submitterId,
            ApartmentId = apartmentId,
            GarageId = garageId
        };

        appeal.SetCreatedAt();
        appeal.RaiseDomainEvent(new AppealSubmittedDomainEvent(appeal.Id, submitterId));

        return appeal;
    }

    /// <summary>
    /// Statusu dəyiş (admin).
    /// </summary>
    public void UpdateStatus(AppealStatus newStatus, Guid updatedByUserId)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;

        if (newStatus == AppealStatus.Resolved)
            ResolvedAt = DateTimeOffset.UtcNow;

        if (newStatus == AppealStatus.Closed)
            ClosedAt = DateTimeOffset.UtcNow;

        SetUpdatedAt();
        RaiseDomainEvent(new AppealStatusChangedDomainEvent(Id, oldStatus, newStatus, updatedByUserId));
    }

    /// <summary>
    /// Prioritet dəyiş (admin).
    /// </summary>
    public void UpdatePriority(AppealPriority newPriority)
    {
        if (Priority == newPriority)
            return;

        Priority = newPriority;
        SetUpdatedAt();
    }

    /// <summary>
    /// Şərh əlavə et (hər iki tərəf).
    /// </summary>
    public AppealComment AddComment(string text, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Şərh boş ola bilməz", nameof(text));

        if (text.Length > 1000)
            throw new ArgumentException("Şərh 1000 simvoldan çox ola bilməz", nameof(text));

        var comment = AppealComment.Create(Id, text.Trim(), userId);
        _comments.Add(comment);
        SetUpdatedAt();

        return comment;
    }
}
