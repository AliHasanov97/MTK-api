using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions;

public static class WarehouseTransactionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("WarehouseTransaction.NotFound", $"Anbar əməliyyatı ID '{id}' tapılmadı");

    public static Error InvalidQuantity =>
        Error.Validation("WarehouseTransaction.InvalidQuantity", "Miqdar 0-dan böyük olmalıdır");

    public static Error InvalidUnitPrice =>
        Error.Validation("WarehouseTransaction.InvalidUnitPrice", "Vahid qiymət 0-dan kiçik ola bilməz");

    public static Error NomenclatureNotFound =>
        Error.Problem("WarehouseTransaction.NomenclatureNotFound", "Nomenklatura tapılmadı");

    public static Error InsufficientStock(decimal available, decimal requested) =>
        Error.Problem(
            "WarehouseTransaction.InsufficientStock",
            $"Kifayət qədər stok yoxdur. Mövcud: {available}, Tələb: {requested}");
}
