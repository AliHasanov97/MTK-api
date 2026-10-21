using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Reports.GetWarehouseMonthlyReport;

public sealed record GetWarehouseMonthlyReportQuery(int Year, int Month)
    : IQuery<WarehouseMonthlyReportResponse>;

/// <summary>
/// "Aylıq hesabat"ın anbar bölməsi. Faktiki qalıq cari stokdur; mədaxil/məxaric
/// isə seçilmiş ayın hərəkətləridir (nümunə sənəddəki izahla eyni məntiq).
/// </summary>
public sealed record WarehouseMonthlyReportResponse(
    decimal CurrentStockQuantity,
    int StockItemCount,
    decimal StockValue,
    decimal ReceiptQuantity,
    int ReceiptCount,
    decimal IssueQuantity,
    int IssueCount,
    int MovementCount,
    IReadOnlyCollection<WarehouseStockRow> Items,
    IReadOnlyCollection<WarehouseMovementRow> Movements);

public sealed record WarehouseStockRow(
    string Code,
    string Name,
    string Unit,
    decimal Quantity,
    decimal? UnitPrice,
    decimal? Value,
    DateTimeOffset? LastTransactionDate);

public sealed record WarehouseMovementRow(
    DateTimeOffset Date,
    string ItemName,
    string Type,
    decimal Quantity,
    decimal? UnitPrice,
    decimal? Value,
    string? Note);
