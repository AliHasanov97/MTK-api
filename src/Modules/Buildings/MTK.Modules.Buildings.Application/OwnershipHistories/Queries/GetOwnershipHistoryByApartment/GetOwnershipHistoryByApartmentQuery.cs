using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.OwnershipHistories.Queries.GetOwnershipHistoryByApartment;

public sealed record GetOwnershipHistoryByApartmentQuery(Guid ApartmentId)
    : IQuery<IReadOnlyCollection<OwnershipHistoryResponse>>;

/// <summary>Bir mülkiyyət transferi — kim nə vaxt köçürüb, əvvəlki/yeni sahib kim idi.</summary>
public sealed record OwnershipHistoryResponse(
    Guid Id,
    Guid? PreviousOwnerId,
    string? PreviousOwnerName,
    Guid NewOwnerId,
    string NewOwnerName,
    DateTime TransferDate);
