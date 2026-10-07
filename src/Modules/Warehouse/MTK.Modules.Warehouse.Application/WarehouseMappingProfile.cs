using AutoMapper;
using MTK.Modules.Warehouse.Application.Nomenclatures.Queries.GetNomenclatureById;
using MTK.Modules.Warehouse.Application.Nomenclatures.Queries.SearchNomenclatures;
using MTK.Modules.Warehouse.Application.Stocks.Queries.GetAllStock;
using MTK.Modules.Warehouse.Application.Stocks.Queries.GetStockByNomenclature;
using MTK.Modules.Warehouse.Application.Transactions.Queries.GetTransactionById;
using MTK.Modules.Warehouse.Application.Transactions.Queries.GetTransactionHistory;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application;

/// <summary>
/// AutoMapper profile for Warehouse module. Response types are positional records,
/// so non-matching destination members are configured via ForCtorParam (ForMember
/// would make AutoMapper look for a parameterless constructor + setters).
/// </summary>
public sealed class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        // Nomenclature mappings — all members match the entity.
        CreateMap<Nomenclature, NomenclatureResponse>();
        CreateMap<Nomenclature, NomenclatureListItem>();

        // WarehouseTransaction mappings — NomenclatureName is joined from the navigation.
        CreateMap<WarehouseTransaction, TransactionResponse>()
            .ForCtorParam(nameof(TransactionResponse.NomenclatureName), o => o.MapFrom(s => s.Nomenclature.Name));
        CreateMap<WarehouseTransaction, TransactionDto>()
            .ForCtorParam(nameof(TransactionDto.NomenclatureName), o => o.MapFrom(s => s.Nomenclature.Name));

        // WarehouseStock mappings — code/name are joined from the Nomenclature
        // navigation; IsLowStock is derived from MinStockLevel.
        CreateMap<WarehouseStock, StockResponse>()
            .ForCtorParam(nameof(StockResponse.NomenclatureCode), o => o.MapFrom(s => s.Nomenclature.Code))
            .ForCtorParam(nameof(StockResponse.NomenclatureName), o => o.MapFrom(s => s.Nomenclature.Name))
            .ForCtorParam(nameof(StockResponse.MinStockLevel), o => o.MapFrom(s => s.Nomenclature.MinStockLevel))
            .ForCtorParam(nameof(StockResponse.IsLowStock), o => o.MapFrom(s =>
                s.Nomenclature.MinStockLevel.HasValue &&
                s.QuantityOnHand < s.Nomenclature.MinStockLevel.Value));
        CreateMap<WarehouseStock, StockDto>()
            .ForCtorParam(nameof(StockDto.NomenclatureCode), o => o.MapFrom(s => s.Nomenclature.Code))
            .ForCtorParam(nameof(StockDto.NomenclatureName), o => o.MapFrom(s => s.Nomenclature.Name));
    }
}
