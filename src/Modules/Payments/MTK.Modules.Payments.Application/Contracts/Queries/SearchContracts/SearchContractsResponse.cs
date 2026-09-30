using MTK.Modules.Payments.Domain.Contracts;

namespace MTK.Modules.Payments.Application.Contracts.Queries.SearchContracts;

public sealed record SearchContractsResponse(
    IReadOnlyCollection<ContractListItem> Contracts,
    int TotalCount,
    int Page,
    int PageSize);

/// <summary>
/// Siyahı üçün yüngül DTO: xidmətlər yüklənmir (aggregate-in kolleksiyasını
/// hər sətir üçün oxumaq lazımsızdır). Xidmətlər lazımdırsa get-by-id istifadə olunur.
/// </summary>
public sealed record ContractListItem(
    Guid Id,
    string Number,
    Guid VendorId,
    string? VendorName,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    ContractStatus Status,
    string Currency,
    bool IsExpired,
    string? Note,
    DateTimeOffset CreatedAt);
