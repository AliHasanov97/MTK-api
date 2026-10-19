using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Queries.SearchAppeals;

public sealed record SearchAppealsResponse(
    List<AppealListItemResponse> Appeals,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record AppealListItemResponse(
    Guid Id,
    string Title,
    AppealCategory Category,
    AppealStatus Status,
    AppealPriority Priority,
    Guid SubmitterId,
    string? SubmitterName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int CommentsCount);
