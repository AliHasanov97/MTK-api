namespace MTK.Common.Domain.Abstractions;

public interface IAuditable
{
    void SetCreatedAt();
    void SetUpdatedAt();
    void SetDeletedAt();

    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? UpdatedAt { get; }
    DateTimeOffset? DeletedAt { get; }
}
