using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Reports.GetWarehouseMonthlyReport;

internal sealed class GetWarehouseMonthlyReportQueryHandler
    : IQueryHandler<GetWarehouseMonthlyReportQuery, WarehouseMonthlyReportResponse>
{
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IWarehouseTransactionRepository _transactionRepository;

    public GetWarehouseMonthlyReportQueryHandler(
        IWarehouseStockRepository stockRepository,
        IWarehouseTransactionRepository transactionRepository)
    {
        _stockRepository = stockRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<WarehouseMonthlyReportResponse>> Handle(
        GetWarehouseMonthlyReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Year is < 2000 or > 2200)
        {
            return Result.Failure<WarehouseMonthlyReportResponse>(new Error(
                "WarehouseMonthlyReport.InvalidYear",
                "İl 2000-2200 aralığında olmalıdır"));
        }

        if (request.Month is < 1 or > 12)
        {
            return Result.Failure<WarehouseMonthlyReportResponse>(new Error(
                "WarehouseMonthlyReport.InvalidMonth",
                "Ay 1-12 aralığında olmalıdır"));
        }

        var start = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMonths(1);

        // Hesabat üçün bütün stok sətirləri lazımdır; repository-nin standart
        // səhifələmə limiti 50-dir, ona görə geniş limit veririk.
        var stocks = await _stockRepository.GetAllStockAsync(
            pageNumber: 1,
            pageSize: 100_000,
            cancellationToken);

        var latestPrices = await _transactionRepository.GetLatestUnitPricesAsync(
            stocks.Select(s => s.NomenclatureId).ToList(),
            cancellationToken);

        var items = stocks
            .Select(stock =>
            {
                var unitPrice = latestPrices.TryGetValue(stock.NomenclatureId, out var price)
                    ? price
                    : (decimal?)null;

                return new WarehouseStockRow(
                    stock.Nomenclature.Code,
                    stock.Nomenclature.Name,
                    UnitLabel(stock.Nomenclature.Unit),
                    stock.QuantityOnHand,
                    unitPrice,
                    unitPrice is null ? null : stock.QuantityOnHand * unitPrice.Value,
                    stock.LastTransactionDate);
            })
            .OrderBy(i => i.Name)
            .ToList();

        var movements = await _transactionRepository.GetByDateRangeAsync(start, end, cancellationToken);

        var movementRows = movements
            .Select(t => new WarehouseMovementRow(
                t.TransactionDate,
                t.Nomenclature.Name,
                t.TransactionType == TransactionType.Receipt ? "Mədaxil" : "Məxaric",
                t.Quantity,
                t.Notes))
            .OrderByDescending(m => m.Date)
            .ToList();

        var receipts = movements.Where(t => t.TransactionType == TransactionType.Receipt).ToList();
        var issues = movements.Where(t => t.TransactionType == TransactionType.Issue).ToList();

        var stockValue = items.Sum(i => i.Value ?? 0);

        return new WarehouseMonthlyReportResponse(
            CurrentStockQuantity: items.Sum(i => i.Quantity),
            StockItemCount: items.Count,
            StockValue: stockValue,
            ReceiptQuantity: receipts.Sum(t => t.Quantity),
            ReceiptCount: receipts.Count,
            IssueQuantity: issues.Sum(t => t.Quantity),
            IssueCount: issues.Count,
            MovementCount: movementRows.Count,
            Items: items,
            Movements: movementRows);
    }

    private static string UnitLabel(Unit unit) => unit switch
    {
        Unit.Piece => "ədəd",
        Unit.Meter => "metr",
        Unit.Kilogram => "kq",
        Unit.Liter => "litr",
        Unit.Set => "komplekt",
        Unit.SquareMeter => "m²",
        Unit.CubicMeter => "m³",
        Unit.Box => "qutu",
        Unit.Package => "paket",
        Unit.Sack => "çuval",
        _ => unit.ToString(),
    };
}
