namespace MTK.Modules.Payments.Application.Nomenclatures.Queries.SearchNomenclatureShadows;

public sealed record SearchNomenclatureShadowsResponse(
    IReadOnlyCollection<NomenclatureShadowSearchResult> Nomenclatures,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record NomenclatureShadowSearchResult(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    int Category,
    int Unit,
    decimal? MinStockLevel,
    bool IsActive);
