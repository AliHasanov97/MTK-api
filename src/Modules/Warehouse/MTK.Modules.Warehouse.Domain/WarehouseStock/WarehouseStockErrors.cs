using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.WarehouseStock;

public static class WarehouseStockErrors
{
    public static Error NotFound(Guid nomenclatureId) =>
       new Error("WarehouseStock.NotFound", $"Nomenklatura ID '{nomenclatureId}' üçün stok qeydi tapılmadı");

    public static Error InsufficientStock(decimal available, decimal requested) =>
      new Error(
            "WarehouseStock.InsufficientStock",
            $"Kifayət qədər stok yoxdur. Mövcud: {available}, Tələb: {requested}");

    public static Error NegativeQuantity =>
      new Error("WarehouseStock.NegativeQuantity", "Stok miqdarı mənfi ola bilməz");
}
