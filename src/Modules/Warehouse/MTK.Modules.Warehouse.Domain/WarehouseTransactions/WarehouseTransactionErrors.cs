using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseTransactions;

public static class WarehouseTransactionErrors
{
    public static Error NotFound(Guid id) =>
        new Error("WarehouseTransaction.NotFound", $"Anbar əməliyyatı ID '{id}' tapılmadı");

    public static Error InvalidQuantity =>
        new Error("WarehouseTransaction.InvalidQuantity", "Miqdar 0-dan böyük olmalıdır");

    public static Error InvalidUnitPrice =>
        new Error("WarehouseTransaction.InvalidUnitPrice", "Vahid qiymət 0-dan kiçik ola bilməz");

    public static Error NomenclatureNotFound =>
        new Error("WarehouseTransaction.NomenclatureNotFound", "Nomenklatura tapılmadı");

    public static Error InsufficientStock(decimal available, decimal requested) =>
        new Error(
            "WarehouseTransaction.InsufficientStock",
            $"Kifayət qədər stok yoxdur. Mövcud: {available}, Tələb: {requested}");
}
