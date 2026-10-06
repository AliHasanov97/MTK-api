using AutoMapper;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application;

/// <summary>
/// AutoMapper profile for Warehouse module
/// </summary>
public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        // Nomenclature mappings
        CreateMap<Nomenclature, Nomenclatures.GetNomenclatureById.NomenclatureResponse>();
        CreateMap<Nomenclature, Nomenclatures.GetAllNomenclatures.NomenclatureDto>();
        CreateMap<Nomenclature, Nomenclatures.SearchNomenclatures.NomenclatureSearchDto>();

        // WarehouseTransaction mappings
        CreateMap<WarehouseTransaction, WarehouseTransactions.GetTransactionById.TransactionResponse>();
        CreateMap<WarehouseTransaction, WarehouseTransactions.GetTransactionHistory.TransactionDto>();

        // WarehouseStock mappings
        CreateMap<WarehouseStock, WarehouseStock.GetStockByNomenclature.StockResponse>();
        CreateMap<WarehouseStock, WarehouseStock.GetAllStock.StockDto>();
    }
}
