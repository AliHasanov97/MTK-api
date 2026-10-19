using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Queries.GetAppealById;

public sealed record AppealDetailResponse(
    Guid Id,
    string Title,
    string Description,
    AppealCategory Category,
    AppealStatus Status,
    AppealPriority Priority,
    Guid SubmitterId,
    string? SubmitterName,
    Guid? ApartmentId,
    string? ApartmentLabel,
    Guid? GarageId,
    string? GarageLabel,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? ClosedAt,
    List<AppealCommentResponse> Comments);

public sealed record AppealCommentResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string Text,
    DateTimeOffset CreatedAt);
